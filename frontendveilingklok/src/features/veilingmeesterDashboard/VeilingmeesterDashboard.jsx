import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";

import {
    getActiveVeiling,
    startVeiling,
    pauseVeiling,
    resumeVeiling,
    stopVeiling,
    placeBid,
    fetchVeilingDagen
} from "../veiling/api/veilingApi.js";

import useLiveVeiling from "./hooks/useLiveVeiling";
import LiveKlok from "./components/LiveKlok";
import QueueList from "./components/QueueList";
import VeilingControls from "./components/VeilingControls";

export default function VeilingmeesterDashboard() {
    const { token, role } = useContext(AuthContext);

    const [veiling, setVeiling] = useState(null);
    const [veildagen, setVeildagen] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!token || role !== "Veilingmeester") return;

        async function loadInit() {
            try {
                setLoading(true);

                const [actief, dagen] = await Promise.all([
                    getActiveVeiling(token).catch(() => null),
                    fetchVeilingDagen(token)
                ]);

                setVeiling(actief);
                setVeildagen(dagen);
            } catch (err) {
                console.error(err);
                setError("Kon gegevens niet laden.");
            } finally {
                setLoading(false);
            }
        }

        loadInit();
    }, [token, role]);

    async function handleStart(selectedDate) {
        if (!selectedDate) {
            alert("Kies eerst een veildatum.");
            return;
        }

        try {
            const v = await startVeiling(token, selectedDate);
            setVeiling(v);
            setError("");
        } catch (err) {
            setError("Kon veiling niet starten: " + err.message);
        }
    }

    async function handlePause() {
        await pauseVeiling(token, veiling.id);
        setVeiling(await getActiveVeiling(token));
    }

    async function handleResume() {
        await resumeVeiling(token, veiling.id);
        setVeiling(await getActiveVeiling(token));
    }

    async function handleStop() {
        await stopVeiling(token, veiling.id);
        setVeiling(null);
    }

    const { lot, queue, loading: liveLoading } = useLiveVeiling(token, veiling?.id);

    return (
        <main className="container py-4">
            <h1 className="h3 mb-4">Veilingmeester Dashboard</h1>

            {error && <div className="alert alert-danger">{error}</div>}

            {!veiling && (
                <div className="card p-3 mb-4 shadow-sm">
                    <h4 className="h5">Start een veiling</h4>

                    <div className="d-flex gap-2 align-items-center">
                        <select
                            id="veildatum-select"
                            className="form-select"
                            defaultValue=""
                            onChange={(e) => handleStart(e.target.value)}
                        >
                            <option value="">Kies veildatum...</option>
                            {veildagen.map(d => (
                                <option key={d} value={d}>{d}</option>
                            ))}
                        </select>
                    </div>
                </div>
            )}

            <VeilingControls
                veiling={veiling}
                onStart={handleStart}
                onPause={handlePause}
                onResume={handleResume}
                onStop={handleStop}
            />

            {loading || liveLoading ? <p>Laden...</p> : null}

            {veiling && (
                <>
                    <LiveKlok lot={lot} />

                    <button
                        className="btn btn-primary mb-4"
                        onClick={() => placeBid(token, veiling.id)}
                    >
                        Koop tegen huidige prijs
                    </button>

                    <QueueList queue={queue} />
                </>
            )}

            {!veiling && !loading && (
                <p className="text-muted">
                    Er is momenteel geen actieve veiling.
                </p>
            )}
        </main>
    );
}
