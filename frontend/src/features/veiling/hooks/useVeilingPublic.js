import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { VeilingPublicApi } from "../api/veilingPublicApi";

const API_BASE = import.meta.env.VITE_API_BASE ?? "";
const HUB_PATH = import.meta.env.VITE_SIGNALR_HUB ?? "/hub/veiling";

function safeArr(x) {
    return Array.isArray(x) ? x : [];
}

function msToSeconds(ms) {
    const n = typeof ms === "number" ? ms : Number(ms ?? 0);
    if (!Number.isFinite(n)) return 0;
    return Math.max(0, Math.ceil(n / 1000));
}

function formatMoney(value) {
    const n = typeof value === "number" ? value : Number(value ?? 0);
    return new Intl.NumberFormat("nl-NL", { style: "currency", currency: "EUR" }).format(Number.isFinite(n) ? n : 0);
}

function normalizePublicDto(dto) {
    if (!dto) return { currentProduct: null, queue: [], bids: [] };
    const currentProduct = dto.currentProduct ?? dto.huidigProduct ?? dto.current ?? dto.huidig ?? null;
    const queue = dto.queue ?? dto.wachtrij ?? dto.items ?? [];
    const bids = dto.bids ?? dto.biedingen ?? dto.recentBids ?? [];
    return { ...dto, currentProduct, queue: safeArr(queue), bids: safeArr(bids) };
}

function getToken() {
    return localStorage.getItem("token") || "";
}

export function useVeilingPublic({ veilingId, isKoper = false }) {
    const [data, setData] = useState(null);
    const [tick, setTick] = useState(null);
    const [loading, setLoading] = useState(true);
    const [placing, setPlacing] = useState(false);
    const [error, setError] = useState("");
    const [toast, setToast] = useState("");

    const connRef = useRef(null);
    const lastBidAtRef = useRef(0);

    const currentProduct = data?.currentProduct ?? null;
    const queue = useMemo(() => safeArr(data?.queue), [data]);
    const bids = useMemo(() => safeArr(data?.bids), [data]);

    const currentPrice = useMemo(() => {
        if (tick?.currentPrice != null) return Number(tick.currentPrice);
        if (currentProduct?.huidigePrijs != null) return Number(currentProduct.huidigePrijs);
        return 0;
    }, [tick, currentProduct]);

    const timeLeftMs = useMemo(() => {
        if (tick?.timeLeftMs != null) return Number(tick.timeLeftMs);
        return 0;
    }, [tick]);

    const timeLeftSec = useMemo(() => msToSeconds(timeLeftMs), [timeLeftMs]);

    const canBid = useMemo(() => {
        const hasProduct = Boolean(currentProduct);
        const active = Boolean(currentProduct?.isActief);
        const running = timeLeftSec > 0;
        return hasProduct && active && running && isKoper && !placing;
    }, [currentProduct, timeLeftSec, isKoper, placing]);

    const load = useCallback(async () => {
        if (!veilingId) return;
        setLoading(true);
        setError("");
        try {
            const raw = await VeilingPublicApi.loadPublic(veilingId);
            setData(normalizePublicDto(raw));
        } catch (e) {
            setData(null);
            setTick(null);
            setError(e?.message || "Fout bij laden veiling");
        } finally {
            setLoading(false);
        }
    }, [veilingId]);

    const connect = useCallback(async () => {
        if (!veilingId) return;

        if (connRef.current) {
            try {
                await connRef.current.stop();
            } catch {}
            connRef.current = null;
        }

        const conn = new HubConnectionBuilder()
            .withUrl(`${API_BASE}${HUB_PATH}`, {
                accessTokenFactory: () => getToken(),
                withCredentials: true,
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Warning)
            .build();

        conn.on("OntvangTick", (payload) => {
            if (!payload) return;
            if (payload.veilingId != null && Number(payload.veilingId) !== Number(veilingId)) return;
            setTick(payload);
        });

        conn.on("OntvangHuidigProduct", (payload) => {
            setData((prev) => {
                const p = prev ? { ...prev } : normalizePublicDto({});
                p.currentProduct = payload ?? null;
                return p;
            });
        });

        conn.on("OntvangWachtrij", (payload) => {
            setData((prev) => {
                const p = prev ? { ...prev } : normalizePublicDto({});
                p.queue = safeArr(payload);
                return p;
            });
        });

        conn.on("OntvangBod", (payload) => {
            if (!payload) return;
            setData((prev) => {
                const p = prev ? { ...prev } : normalizePublicDto({});
                p.bids = [payload, ...safeArr(p.bids)].slice(0, 50);
                return p;
            });
        });

        conn.on("OntvangBiedingen", (payload) => {
            setData((prev) => {
                const p = prev ? { ...prev } : normalizePublicDto({});
                p.bids = safeArr(payload);
                return p;
            });
        });

        await conn.start();
        await conn.invoke("JoinVeiling", Number(veilingId));

        connRef.current = conn;
    }, [veilingId]);

    useEffect(() => {
        load().catch(() => {});
        connect().catch((e) => setError(e?.message || "SignalR connect mislukt"));

        return () => {
            const c = connRef.current;
            connRef.current = null;
            if (c) c.stop().catch(() => {});
        };
    }, [load, connect]);

    const placeBid = useCallback(
        async (price = null) => {
            if (!veilingId) return;

            setError("");
            setToast("");

            const now = Date.now();
            if (now - lastBidAtRef.current < 600) return;

            if (!isKoper) {
                setError("Log in als koper om te bieden.");
                return;
            }

            if (!currentProduct?.isActief) {
                setError("Er is momenteel geen actief product.");
                return;
            }

            if (timeLeftSec <= 0) {
                setError("Veiling is niet actief (geen tijd meer).");
                return;
            }

            setPlacing(true);
            lastBidAtRef.current = now;

            try {
                const result = await VeilingPublicApi.placeBid(veilingId, price);
                const msg = (result && typeof result === "object" && (result.message || result.Message)) || "Bod geplaatst.";
                setToast(msg);
                await load();
            } catch (e) {
                setError(e?.message || "Bieden mislukt");
            } finally {
                setPlacing(false);
            }
        },
        [veilingId, isKoper, currentProduct, timeLeftSec, load]
    );

    const ui = useMemo(
        () => ({
            priceText: formatMoney(currentPrice),
            timeText: `${timeLeftSec}s`,
        }),
        [currentPrice, timeLeftSec]
    );

    return {
        loading,
        placing,
        error,
        toast,
        data,
        tick,
        currentProduct,
        queue,
        bids,
        timeLeftSec,
        currentPrice,
        canBid,
        ui,
        actions: { refresh: load, placeBid },
    };
}
