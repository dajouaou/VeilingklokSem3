import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";

import {
    getActiveVeiling,
    startVeiling,
    pauseVeiling,
    resumeVeiling,
    stopVeiling,
    placeBid
} from "./api/veilingApi";

import useLiveVeiling from "./hooks/useLiveVeiling";
import LiveKlok from "./components/LiveKlok";
import QueueList from "./components/QueueList";
import VeilingControls from "./components/VeilingControls";

export default function VeilingmeesterDashboard() {
    const { token } = useContext(AuthContext);

    const [veiling, setVeiling] = useState(null);

    useEffect(() => {
        loadActiveVeiling();
    }, []);

    async function loadActiveVeiling() {
        try {
            const v = await getActiveVeiling(token);
            setVeiling(v);
        } catch {
            setVeiling(null);
        }
    }

    async function handleStart() {
        const date = prompt("Welke veildatum wil je starten? (YYYY-MM-DD)");
        if (!date) return;

        const v = await startVeiling(token, date);
        setVeiling(v);
    }

    async function handlePause() {
        await pauseVeiling(token, veiling.id);
        loadActiveVeiling();
    }

    async function handleResume() {
        await resumeVeiling(token, veiling.id);
        loadActiveVeiling();
    }

    async function handleStop() {
        await stopVeiling(token, veiling.id);
        setVeiling(null);
    }

    // Live updates via SignalR
    const { lot, queue, loading } = useLiveVeiling(token, veiling?.id);

    return (
        <main className="container py-4">
            <h1 className="h3 mb-4">Veilingmeester Dashboard</h1>

            <VeilingControls
                veiling={veiling}
                onStart={handleStart}
                onPause={handlePause}
                onResume={handleResume}
                onStop={handleStop}
            />

            {loading && <p>Laden...</p>}

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

            {!veiling && <p>Er is momenteel geen actieve veiling.</p>}
        </main>
    );
}
