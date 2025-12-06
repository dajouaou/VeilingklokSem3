import { useEffect, useState, useCallback } from "react";
import {
    getDashboard,
    startVeiling,
    nextProduct,
    closeCurrent,
} from "../api/VMApi";

export default function useVMDashboard(veilingId) {
    const [dashboard, setDashboard] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const loadDashboard = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);

            const data = await getDashboard(veilingId);

            // Backend stuurt:
            // {
            //   veilingId,
            //   vmNaam,
            //   current,
            //   queue,
            //   audit
            // }

            setDashboard({
                currentProduct: data.current ?? null,
                queue: data.queue ?? [],
                bids: data.current?.bids ?? [],
                audit: data.audit ?? [],
                raw: data
            });

        } catch (err) {
            console.error("Dashboard load error:", err);
            setError("Kon dashboard niet laden");
        } finally {
            setLoading(false);
        }
    }, [veilingId]);

    useEffect(() => {
        loadDashboard();
    }, [loadDashboard]);

    return {
        dashboard,
        loading,
        error,

        currentProduct: dashboard?.currentProduct || null,
        queue: dashboard?.queue || [],
        bids: dashboard?.bids || [],
        audit: dashboard?.audit || [],

        startVeiling: async () => {
            await startVeiling(veilingId);
            await loadDashboard();
        },

        nextProduct: async () => {
            await nextProduct(veilingId);
            await loadDashboard();
        },

        closeProduct: async () => {
            await closeCurrent(veilingId);
            await loadDashboard();
        },
    };
}
