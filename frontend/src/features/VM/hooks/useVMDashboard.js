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

    const loadDashboard = useCallback(async () => {
        setLoading(true);
        const data = await getDashboard(veilingId);
        setDashboard(data);
        setLoading(false);
    }, [veilingId]);

    useEffect(() => {
        loadDashboard();
    }, [loadDashboard]);

    return {
        dashboard,
        loading,

        currentProduct: dashboard?.currentProduct || null,
        queue: dashboard?.queue || [],
        bids: dashboard?.bids || [],

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
