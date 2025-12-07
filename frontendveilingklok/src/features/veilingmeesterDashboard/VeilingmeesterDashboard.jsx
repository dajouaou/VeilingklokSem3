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
import WachtrijLijst from "./components/WachtrijLijst";
import VeilingControls from "./components/VeilingControls";
import BiedingenLijst from "./components/BiedingenLijst";
import AuditLijst from "./components/AuditLijst";
import VeilingInformatie from "./components/VeilingInformatie";

export default function VeilingmeesterDashboard() {
    const { token, role } = useContext(AuthContext);

    const [veiling, setVeiling] = useState(null);
    const [veildagen, setVeildagen] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [gekozenDatum, setGekozenDatum] = useState("");
    const [startTijd, setStartTijd] = useState("09:00");

    // Live data via SignalR
    const {
        lot,
        queue,
        bids,
        audit,
        loading: liveLoading
    } = useLiveVeiling(token, veiling?.id);

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

    async function handleStart() {
        if (!gekozenDatum) {
            alert("Kies eerst een veildatum.");
            return;
        }

        if (!startTijd) {
            alert("Kies eerst een starttijd.");
            return;
        }

        try {
            const v = await startVeiling(token, gekozenDatum, startTijd);
            setVeiling(v);
            setError("");
        } catch (err) {
            console.error(err);
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

    return (
        <main className="container py-4">
            <h1 className="h3 mb-4">Veilingmeester Dashboard</h1>

            {error && <div className="alert alert-danger">{error}</div>}

            {!veiling && (
                <div className="card p-3 mb-4 shadow-sm">
                    <h4 className="h5">Veiling aanmaken</h4>

                    <div className="d-flex gap-2 align-items-center mt-2">
                        <select
                            className="form-select"
                            value={gekozenDatum}
                            onChange={(e) => setGekozenDatum(e.target.value)}
                        >
                            <option value="">Kies veildatum...</option>
                            {veildagen.map((d) => (
                                <option key={d} value={d}>
                                    {d}
                                </option>
                            ))}
                        </select>

                        <input
                            type="time"
                            className="form-control"
                            value={startTijd}
                            onChange={(e) => setStartTijd(e.target.value)}
                        />
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

            {(loading || liveLoading) && <p>Laden...</p>}

            {veiling && (
                <>
                    <VeilingInformatie gegevens={veiling} />

                    <LiveKlok lot={lot} />

                    <button
                        className="btn btn-primary mb-4"
                        onClick={() => placeBid(token, veiling.id)}
                    >
                        Koop tegen huidige prijs
                    </button>

                    <WachtrijLijst wachtrij={queue} />

                    <div className="row mt-4">
                        <div className="col-md-6">
                            <BiedingenLijst biedingen={bids} />
                        </div>
                        <div className="col-md-6">
                            <AuditLijst audit={audit} />
                        </div>
                    </div>
                </>
            )}

            {!veiling && !loading && (
                <p className="text-muted">Er is momenteel geen actieve veiling.</p>
            )}
        </main>
    );
}
