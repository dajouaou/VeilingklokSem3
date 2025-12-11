// src/features/veiling/hooks/useVeilingRealtime.js
import { useEffect, useState, useRef } from "react";
import * as signalR from "@microsoft/signalr";

const API_BASE = import.meta.env.VITE_API_BASE;
const HUB_PATH = import.meta.env.VITE_SIGNALR_HUB || "/hubs/auction";

export default function useVeilingRealtime(veilingId) {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [currentProduct, setCurrentProduct] = useState(null);
    const [queue, setQueue] = useState([]);
    const [bids, setBids] = useState([]);

    const [timeLeftMs, setTimeLeftMs] = useState(0);
    const [currentPrice, setCurrentPrice] = useState(0);
    const [highestBidderId, setHighestBidderId] = useState(null);

    const [userId, setUserId] = useState(null);
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    const connectionRef = useRef(null);

    useEffect(() => {
        try {
            const raw = localStorage.getItem("authUser");
            if (!raw) {
                setIsLoggedIn(false);
                setUserId(null);
                return;
            }

            const user = JSON.parse(raw);
            setIsLoggedIn(true);
            setUserId(user?.id ?? user?.userId ?? null);
        } catch {
            setIsLoggedIn(false);
            setUserId(null);
        }
    }, []);

    useEffect(() => {
        let isCancelled = false;

        async function loadInitial() {
            if (!veilingId || !API_BASE) return;

            setLoading(true);
            setError("");

            try {
                const res = await fetch(
                    `${API_BASE}/api/veilingen/${veilingId}/public`
                );

                if (!res.ok) {
                    const text = await res.text();
                    if (!isCancelled) {
                        setError(text || "Kon veiling niet laden.");
                        setLoading(false);
                    }
                    return;
                }

                const dto = await res.json();
                if (isCancelled) return;

                setCurrentProduct(dto.currentProduct ?? null);
                setQueue(dto.queue ?? []);
                setBids((dto.bids ?? []).sort(
                    (a, b) =>
                        new Date(b.placedAtUtc).getTime() -
                        new Date(a.placedAtUtc).getTime()
                ));

                setTimeLeftMs(dto.timeLeftMs ?? 0);
                setCurrentPrice(dto.currentPrice ?? 0);
                setHighestBidderId(dto.highestBidderId ?? null);

                setLoading(false);
            } catch {
                if (!isCancelled) {
                    setError("Er ging iets mis bij het laden van de veiling.");
                    setLoading(false);
                }
            }
        }

        loadInitial();

        return () => {
            isCancelled = true;
        };
    }, [veilingId]);

    useEffect(() => {
        if (!veilingId || !API_BASE) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_BASE}${HUB_PATH}?veilingId=${veilingId}`, {
                withCredentials: false,
            })
            .withAutomaticReconnect()
            .build();

        connectionRef.current = connection;

        async function startConnection() {
            try {
                await connection.start();
            } catch {
                setError("Kon geen realtime verbinding maken.");
            }
        }

        connection.on("ClockTick", (payload) => {
            if (!payload) return;
            if (payload.veilingId && String(payload.veilingId) !== String(veilingId)) return;

            if (typeof payload.timeLeftMs === "number") {
                setTimeLeftMs(payload.timeLeftMs);
            }
            if (typeof payload.currentPrice === "number") {
                setCurrentPrice(payload.currentPrice);
            }
        });

        connection.on("BidPlaced", (bidDto) => {
            if (!bidDto) return;
            setBids((prev) => {
                const updated = [bidDto, ...prev];
                return updated.sort(
                    (a, b) =>
                        new Date(b.placedAtUtc).getTime() -
                        new Date(a.placedAtUtc).getTime()
                );
            });

            if (typeof bidDto.amount === "number") {
                setCurrentPrice(bidDto.amount);
            }
            if (bidDto.koperId) {
                setHighestBidderId(bidDto.koperId);
            }
        });

        connection.on("CurrentProductChanged", (dto) => {
            setCurrentProduct(dto?.currentProduct ?? null);
            setBids([]);
            setTimeLeftMs(dto?.timeLeftMs ?? 0);
            setCurrentPrice(dto?.currentPrice ?? 0);
        });

        connection.on("QueueUpdated", (items) => {
            setQueue(items ?? []);
        });

        startConnection();

        return () => {
            if (connectionRef.current) {
                connectionRef.current.stop().catch(() => {});
                connectionRef.current = null;
            }
        };
    }, [veilingId]);

    async function placeBid() {
        if (!isLoggedIn || !userId) {
            setError("Je moet ingelogd zijn om te kunnen bieden.");
            return;
        }

        try {
            setError("");

            const res = await fetch(
                `${API_BASE}/api/veilingen/${veilingId}/bids?koperId=${userId}`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({}),
                }
            );

            if (!res.ok) {
                const text = await res.text();
                setError(text || "Bod plaatsen is mislukt.");
                return;
            }
        } catch {
            setError("Er ging iets mis bij het plaatsen van je bod.");
        }
    }

    return {
        loading,
        error,
        currentProduct,
        queue,
        bids,
        timeLeftMs,
        currentPrice,
        placeBid,
        isLoggedIn,
        highestBidderId,
        userId,
    };
}
