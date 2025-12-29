import { useEffect, useMemo, useRef, useState } from "react";
import { VeilingmeesterApi } from "../api/veilingmeesterApi";

export function useVeilingmeesterDashboard() {
    const [activeVeiling, setActiveVeiling] = useState(null);
    const [selectedVeilingId, setSelectedVeilingId] = useState(1);
    const [dashboard, setDashboard] = useState(null);

    const [loading, setLoading] = useState(true);
    const [loadingDashboard, setLoadingDashboard] = useState(false);
    const [error, setError] = useState("");

    const currentVeilingId = useMemo(() => {
        if (activeVeiling?.id) return activeVeiling.id;
        if (dashboard?.veilingId) return dashboard.veilingId;
        return selectedVeilingId;
    }, [activeVeiling, dashboard, selectedVeilingId]);

    const mountedRef = useRef(false);

    async function loadActive() {
        setLoading(true);
        setError("");
        try {
            const a = await VeilingmeesterApi.getActieveVeiling();
            setActiveVeiling(a);
            if (a?.id) setSelectedVeilingId(a.id);
            if (a?.id) {
                await loadDashboard(a.id);
            } else {
                setDashboard(null);
            }
        } catch (e) {
            setError(e.message || "Fout bij laden actieve veiling");
            setDashboard(null);
        } finally {
            setLoading(false);
        }
    }

    async function loadDashboard(veilingId) {
        if (!veilingId) return;
        setLoadingDashboard(true);
        setError("");
        try {
            const d = await VeilingmeesterApi.getDashboard(veilingId);
            setDashboard(d);
        } catch (e) {
            setError(e.message || "Fout bij laden dashboard");
            setDashboard(null);
        } finally {
            setLoadingDashboard(false);
        }
    }

    async function ensureDashboard() {
        const id = currentVeilingId;
        if (!id) return;
        await loadDashboard(id);
    }

    useEffect(() => {
        if (mountedRef.current) return;
        mountedRef.current = true;
        loadActive().then(() => {});
    }, []);

    async function selectVeilingAndLoad(id) {
        setSelectedVeilingId(id);
        await loadDashboard(id);
    }

    async function start() {
        await action(() => VeilingmeesterApi.startVeiling(currentVeilingId));
    }

    async function pause() {
        await action(() => VeilingmeesterApi.pauseVeiling(currentVeilingId));
    }

    async function resume() {
        await action(() => VeilingmeesterApi.resumeVeiling(currentVeilingId));
    }

    async function stop() {
        await action(() => VeilingmeesterApi.stopVeiling(currentVeilingId));
    }

    async function reset() {
        await action(() => VeilingmeesterApi.resetVeiling(currentVeilingId));
    }

    async function next() {
        await action(() => VeilingmeesterApi.activateNext(currentVeilingId));
    }

    async function closeCurrent() {
        await action(() => VeilingmeesterApi.closeCurrent(currentVeilingId));
    }

    async function skip(veilingProductId) {
        await action(() => VeilingmeesterApi.skipProduct(currentVeilingId, veilingProductId));
    }

    async function reorder(orderedIds) {
        await action(() => VeilingmeesterApi.reorderQueue(currentVeilingId, orderedIds));
    }

    async function action(fn) {
        setError("");
        try {
            const d = await fn();
            if (d && typeof d === "object" && "veilingId" in d) {
                setDashboard(d);
                return;
            }
            await ensureDashboard();
        } catch (e) {
            setError(e.message || "Actie mislukt");
        }
    }

    return {
        loading,
        loadingDashboard,
        error,
        activeVeiling,
        selectedVeilingId,
        setSelectedVeilingId,
        currentVeilingId,
        dashboard,
        loadActive,
        loadDashboard,
        selectVeilingAndLoad,
        actions: {
            start,
            pause,
            resume,
            stop,
            reset,
            next,
            closeCurrent,
            skip,
            reorder,
            refresh: ensureDashboard,
        },
    };
}
