// src/hooks/useVMDashboard.js

import { useEffect, useState, useCallback } from "react";
import {
    getDashboard,
    startVeiling as apiStartVeiling,
    nextProduct as apiNextProduct,
    closeCurrent as apiCloseCurrent,
} from "../api/VMApi";

export default function useVMDashboard(veilingId = 1) {
    const [dashboard, setDashboard] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // -----------------------------
    // Dashboard laden
    // -----------------------------
    const loadDashboard = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const data = await getDashboard(veilingId);

            // Backend stuurt ongeveer:
            // { veilingId, vmNaam, current, queue, audit }
            setDashboard({
                currentProduct: data?.current ?? null,
                queue: data?.queue ?? [],
                bids: data?.current?.bids ?? [],
                audit: data?.audit ?? [],
                raw: data,
            });
        } catch (err) {
            console.error("Dashboard load error:", err);
            setError(err.message || "Kon dashboard niet laden");
        } finally {
            setLoading(false);
        }
    }, [veilingId]);

    useEffect(() => {
        loadDashboard();
    }, [loadDashboard]);

    // -----------------------------
    // Acties: start / next / close
    // -----------------------------
    const handleStart = useCallback(async () => {
        setError(null);
        try {
            await apiStartVeiling(veilingId);
            await loadDashboard();
        } catch (err) {
            console.error("Start veiling error:", err);
            setError(err.message || "Kon veiling niet starten");
        }
    }, [veilingId, loadDashboard]);

    const handleNext = useCallback(async () => {
        setError(null);
        try {
            await apiNextProduct(veilingId);
            await loadDashboard();
        } catch (err) {
            console.error("Next product error:", err);
            // hier komt bv. "Geen volgende producten."
            setError(err.message || "Kon volgend product niet laden");
        }
    }, [veilingId, loadDashboard]);

    const handleClose = useCallback(async () => {
        setError(null);
        try {
            await apiCloseCurrent(veilingId);
            await loadDashboard();
        } catch (err) {
            console.error("Close product error:", err);
            setError(err.message || "Kon product niet sluiten");
        }
    }, [veilingId, loadDashboard]);

    // -----------------------------
    // Handige shorthands voor UI
    // -----------------------------
    const currentProduct = dashboard?.currentProduct ?? null;
    const queue = dashboard?.queue ?? [];
    const bids = dashboard?.bids ?? [];
    const audit = dashboard?.audit ?? [];

    return {
        dashboard,
        loading,
        error,

        currentProduct,
        queue,
        bids,
        audit,

        startVeiling: handleStart,
        nextProduct: handleNext,
        closeProduct: handleClose,

        reload: loadDashboard,
    };
}
