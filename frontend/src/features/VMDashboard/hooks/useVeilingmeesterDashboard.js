import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { VeilingmeesterApi } from "../api/veilingmeesterApi";

export function useVeilingmeesterDashboard() {
    const [activeVeiling, setActiveVeiling] = useState(null);
    const [selectedVeilingId, setSelectedVeilingId] = useState(1);
    const [dashboard, setDashboard] = useState(null);

    const [loading, setLoading] = useState(true);
    const [loadingDashboard, setLoadingDashboard] = useState(false);
    const [actionLoading, setActionLoading] = useState(false);
    const [error, setError] = useState("");

    const abortRef = useRef(null);
    const mountedRef = useRef(false);
    const lastDashboardReqIdRef = useRef(0);

    const currentVeilingId = useMemo(() => {
        if (activeVeiling?.id) return activeVeiling.id;
        if (dashboard?.veilingId) return dashboard.veilingId;
        return selectedVeilingId;
    }, [activeVeiling, dashboard, selectedVeilingId]);

    const status = dashboard?.status ?? activeVeiling?.status ?? null;
    const hasCurrentProduct = Boolean(dashboard?.current?.id || dashboard?.current?.veilingProductId);
    const hasQueue = Array.isArray(dashboard?.queue) && dashboard.queue.length > 0;
    const isRunning = status === "Running";
    const isPaused = status === "Paused";
    const isFinished = status === "Finished";
    const isScheduled = status === "Scheduled";

    const canStart = Boolean(currentVeilingId) && !actionLoading && (isScheduled || status == null);
    const canPause = Boolean(currentVeilingId) && !actionLoading && isRunning;
    const canResume = Boolean(currentVeilingId) && !actionLoading && isPaused;
    const canStop = Boolean(currentVeilingId) && !actionLoading && !isFinished && status != null;
    const canReset = Boolean(currentVeilingId) && !actionLoading;
    const canNext = Boolean(currentVeilingId) && !actionLoading && !isFinished && !isScheduled;
    const canCloseCurrent = Boolean(currentVeilingId) && !actionLoading && hasCurrentProduct && !isFinished && !isScheduled;
    const canSkipQueueItem = Boolean(currentVeilingId) && !actionLoading && !isFinished && !isScheduled;
    const canReorder = Boolean(currentVeilingId) && !actionLoading && !isRunning && !isPaused && hasQueue;

    const cancelInFlight = useCallback(() => {
        if (abortRef.current) {
            abortRef.current.abort();
            abortRef.current = null;
        }
    }, []);

    const setFriendlyError = useCallback((e, fallback) => {
        const msg = e?.message || fallback;
        setError(msg);
    }, []);

    const loadDashboard = useCallback(
        async (veilingId) => {
            if (!veilingId) return;

            const reqId = ++lastDashboardReqIdRef.current;
            setLoadingDashboard(true);
            setError("");

            try {
                const d = await VeilingmeesterApi.getDashboard(veilingId);
                if (reqId !== lastDashboardReqIdRef.current) return;
                setDashboard(d);
            } catch (e) {
                if (reqId !== lastDashboardReqIdRef.current) return;
                setDashboard(null);
                setFriendlyError(e, "Fout bij laden dashboard");
            } finally {
                if (reqId === lastDashboardReqIdRef.current) setLoadingDashboard(false);
            }
        },
        [setFriendlyError]
    );

    const loadActive = useCallback(async () => {
        setLoading(true);
        setError("");
        try {
            const a = await VeilingmeesterApi.getActieveVeiling();
            setActiveVeiling(a);
            if (a?.id) {
                setSelectedVeilingId(a.id);
                await loadDashboard(a.id);
            } else {
                setDashboard(null);
            }
        } catch (e) {
            setDashboard(null);
            setFriendlyError(e, "Fout bij laden actieve veiling");
        } finally {
            setLoading(false);
        }
    }, [loadDashboard, setFriendlyError]);

    const ensureDashboard = useCallback(async () => {
        const id = currentVeilingId;
        if (!id) return;
        await loadDashboard(id);
    }, [currentVeilingId, loadDashboard]);

    useEffect(() => {
        if (mountedRef.current) return;
        mountedRef.current = true;
        loadActive().then(() => {});
        return () => cancelInFlight();
    }, [loadActive, cancelInFlight]);

    const selectVeilingAndLoad = useCallback(
        async (id) => {
            setSelectedVeilingId(id);
            setActiveVeiling(null);
            await loadDashboard(id);
        },
        [loadDashboard]
    );

    const action = useCallback(
        async (fn) => {
            if (!currentVeilingId) {
                setError("Geen veiling geselecteerd.");
                return null;
            }

            setActionLoading(true);
            setError("");

            try {
                const res = await fn();

                if (res && typeof res === "object") {
                    if ("veilingId" in res) {
                        setDashboard(res);
                        return res;
                    }
                    if ("success" in res && res.success === false) {
                        setFriendlyError({ message: res.message || "Actie niet toegestaan" }, "Actie niet toegestaan");
                        return null;
                    }
                }

                await ensureDashboard();
                return res;
            } catch (e) {
                setFriendlyError(e, "Actie mislukt");
                return null;
            } finally {
                setActionLoading(false);
            }
        },
        [currentVeilingId, ensureDashboard, setFriendlyError]
    );

    const start = useCallback(async () => action(() => VeilingmeesterApi.startVeiling(currentVeilingId)), [action, currentVeilingId]);
    const pause = useCallback(async () => action(() => VeilingmeesterApi.pauseVeiling(currentVeilingId)), [action, currentVeilingId]);
    const resume = useCallback(async () => action(() => VeilingmeesterApi.resumeVeiling(currentVeilingId)), [action, currentVeilingId]);
    const stop = useCallback(async () => action(() => VeilingmeesterApi.stopVeiling(currentVeilingId)), [action, currentVeilingId]);
    const reset = useCallback(async () => action(() => VeilingmeesterApi.resetVeiling(currentVeilingId)), [action, currentVeilingId]);
    const next = useCallback(async () => action(() => VeilingmeesterApi.activateNext(currentVeilingId)), [action, currentVeilingId]);
    const closeCurrent = useCallback(async () => action(() => VeilingmeesterApi.closeCurrent(currentVeilingId)), [action, currentVeilingId]);

    const skip = useCallback(
        async (veilingProductId) => action(() => VeilingmeesterApi.skipProduct(currentVeilingId, veilingProductId)),
        [action, currentVeilingId]
    );

    const reorder = useCallback(
        async (orderedIds) => action(() => VeilingmeesterApi.reorderQueue(currentVeilingId, orderedIds)),
        [action, currentVeilingId]
    );

    return {
        loading,
        loadingDashboard,
        actionLoading,
        error,
        activeVeiling,
        selectedVeilingId,
        setSelectedVeilingId,
        currentVeilingId,
        dashboard,
        status,
        hasCurrentProduct,
        hasQueue,
        capabilities: {
            canStart,
            canPause,
            canResume,
            canStop,
            canReset,
            canNext,
            canCloseCurrent,
            canSkipQueueItem,
            canReorder,
        },
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
