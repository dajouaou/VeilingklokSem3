// src/features/VM/hooks/useVMDashboard.js

import { useEffect, useState } from "react";
import {
    getDashboard,
    startVeiling as apiStartVeiling,
    nextProduct as apiNextProduct,
    closeCurrent as apiCloseCurrent,
} from  "../api/VMApi";

function getErrorMessage(err, fallback) {
    if (!err) return fallback;

    const status = err.status ?? err?.response?.status;
    const backendMessage = err.message;

    if (status === 404) {
        return backendMessage || "Veiling niet gevonden.";
    }

    if (status === 409) {
        return backendMessage || "Actie is niet toegestaan in de huidige status.";
    }

    if (typeof err === "string") return err;
    if (backendMessage) return backendMessage;

    return fallback;
}

function buildViewModel(data) {
    const current = data?.current ?? null;
    const queue = data?.queue ?? [];
    const bids = current?.bids ?? [];
    const audit = data?.audit ?? [];

    const productenInQueue = data?.productenInQueue ?? queue.length ?? 0;
    const verkochteProducten = data?.verkochteProducten ?? 0;
    const totalBidsCurrentProduct = bids.length ?? 0;
    const totaalBiedingenVeiling =
        data?.totaalBiedingen ?? totalBidsCurrentProduct;

    const totaalProducten =
        data?.totaalProducten ??
        productenInQueue +
        verkochteProducten +
        (current ? 1 : 0);

    return {
        currentProduct: current,
        queue,
        bids,
        audit,
        veilingNaam: data?.veilingNaam ?? null,
        locatie: data?.locatie ?? null,
        vmNaam: data?.vmNaam ?? null,
        status: data?.status ?? null,
        startTijdUtc: data?.startTijdUtc ?? null,
        eindTijdUtc: data?.eindTijdUtc ?? null,
        totalBidsCurrentProduct,
        totaalBiedingenVeiling,
        productenInQueue,
        verkochteProducten,
        totaalProducten,
    };
}

export default function useVMDashboard(veilingId = 1) {
    const [loading, setLoading] = useState(true);
    const [hasLoadedOnce, setHasLoadedOnce] = useState(false);
    const [error, setError] = useState(null);

    const [currentProduct, setCurrentProduct] = useState(null);
    const [queue, setQueue] = useState([]);
    const [bids, setBids] = useState([]);
    const [audit, setAudit] = useState([]);
    const [raw, setRaw] = useState(null);

    const [veilingNaam, setVeilingNaam] = useState(null);
    const [locatie, setLocatie] = useState(null);
    const [vmNaam, setVmNaam] = useState(null);
    const [status, setStatus] = useState(null);
    const [startTijdUtc, setStartTijdUtc] = useState(null);
    const [eindTijdUtc, setEindTijdUtc] = useState(null);

    const [totalBidsCurrentProduct, setTotalBidsCurrentProduct] = useState(0);
    const [totaalBiedingenVeiling, setTotaalBiedingenVeiling] = useState(0);
    const [productenInQueue, setProductenInQueue] = useState(0);
    const [verkochteProducten, setVerkochteProducten] = useState(0);
    const [totaalProducten, setTotaalProducten] = useState(0);

    function applyView(data) {
        const view = buildViewModel(data);

        setCurrentProduct(view.currentProduct);
        setQueue(view.queue);
        setBids(view.bids);
        setAudit(view.audit);

        setVeilingNaam(view.veilingNaam);
        setLocatie(view.locatie);
        setVmNaam(view.vmNaam);
        setStatus(view.status);
        setStartTijdUtc(view.startTijdUtc);
        setEindTijdUtc(view.eindTijdUtc);

        setTotalBidsCurrentProduct(view.totalBidsCurrentProduct);
        setTotaalBiedingenVeiling(view.totaalBiedingenVeiling);
        setProductenInQueue(view.productenInQueue);
        setVerkochteProducten(view.verkochteProducten);
        setTotaalProducten(view.totaalProducten);

        setRaw(data ?? null);
        setHasLoadedOnce(true);
    }

    async function loadDashboard() {
        setLoading(true);
        setError(null);

        try {
            const data = await getDashboard(veilingId);
            applyView(data);
        } catch (err) {
            console.error("Dashboard load error:", err);
            setError(getErrorMessage(err, "Kon dashboard niet laden."));
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadDashboard();
    }, [veilingId]);

    async function handleStartVeiling() {
        setError(null);
        try {
            const data = await apiStartVeiling(veilingId);
            applyView(data);
        } catch (err) {
            console.error("Start veiling error:", err);
            setError(getErrorMessage(err, "Kon veiling niet starten."));
        }
    }

    async function handleNextProduct() {
        setError(null);
        try {
            const data = await apiNextProduct(veilingId);
            applyView(data);
        } catch (err) {
            console.error("Next product error:", err);
            setError(getErrorMessage(err, "Kon volgend product niet laden."));
        }
    }

    async function handleCloseProduct() {
        setError(null);
        try {
            const data = await apiCloseCurrent(veilingId);
            applyView(data);
        } catch (err) {
            console.error("Close product error:", err);
            setError(getErrorMessage(err, "Kon product niet sluiten."));
        }
    }

    const initialLoading = loading && !hasLoadedOnce;
    const isRefreshing = loading && hasLoadedOnce;

    const queueCount = productenInQueue ?? (queue ? queue.length : 0);
    const hasCurrent = !!currentProduct;

    const canStart = !hasCurrent && queueCount > 0;
    const canNext = hasCurrent && queueCount > 0;
    const canClose = hasCurrent;

    return {
        loading,
        initialLoading,
        isRefreshing,
        error,

        veilingId,
        veilingNaam,
        locatie,
        vmNaam,
        status,
        startTijdUtc,
        eindTijdUtc,

        currentProduct,
        queue,
        bids,
        audit,

        totalBidsCurrentProduct,
        totaalBiedingenVeiling,
        productenInQueue,
        verkochteProducten,
        totaalProducten,

        canStart,
        canNext,
        canClose,

        startVeiling: handleStartVeiling,
        nextProduct: handleNextProduct,
        closeProduct: handleCloseProduct,
        reload: loadDashboard,
        raw,
    };
}
