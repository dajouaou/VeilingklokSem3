// src/hooks/useVMDashboard.js

import { useEffect, useState } from "react";
import {
    getDashboard,
    startVeiling as apiStartVeiling,
    nextProduct as apiNextProduct,
    closeCurrent as apiCloseCurrent,
} from "../api/VMApi";

function getErrorMessage(err, fallback) {
    if (!err) return fallback;
    if (typeof err === "string") return err;
    if (err.message) return err.message;
    return fallback;
}

export default function useVMDashboard(veilingId = 1) {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [currentProduct, setCurrentProduct] = useState(null);
    const [queue, setQueue] = useState([]);
    const [bids, setBids] = useState([]);
    const [audit, setAudit] = useState([]);
    const [raw, setRaw] = useState(null);

    async function loadDashboard() {
        setLoading(true);
        setError(null);

        try {
            const data = await getDashboard(veilingId);

            setCurrentProduct(data?.current ?? null);
            setQueue(data?.queue ?? []);
            setBids(data?.current?.bids ?? []);
            setAudit(data?.audit ?? []);
            setRaw(data ?? null);
        } catch (err) {
            console.error("Dashboard load error:", err);
            setError(getErrorMessage(err, "Kon dashboard niet laden"));
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
            await apiStartVeiling(veilingId);
            await loadDashboard();
        } catch (err) {
            console.error("Start veiling error:", err);
            setError(getErrorMessage(err, "Kon veiling niet starten"));
        }
    }

    async function handleNextProduct() {
        setError(null);
        try {
            await apiNextProduct(veilingId);
            await loadDashboard();
        } catch (err) {
            console.error("Next product error:", err);
            setError(getErrorMessage(err, "Kon volgend product niet laden"));
        }
    }

    async function handleCloseProduct() {
        setError(null);
        try {
            await apiCloseCurrent(veilingId);
            await loadDashboard();
        } catch (err) {
            console.error("Close product error:", err);
            setError(getErrorMessage(err, "Kon product niet sluiten"));
        }
    }

    return {
        loading,
        error,
        currentProduct,
        queue,
        bids,
        audit,
        raw,
        startVeiling: handleStartVeiling,
        nextProduct: handleNextProduct,
        closeProduct: handleCloseProduct,
        reload: loadDashboard,
    };
}
