import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";
import { fetchVeilingDagen, startVeiling } from "../veiling/api/veilingApi";

export default function PlanVeiling() {
    const { token } = useContext(AuthContext);

    const [dagen, setDagen] = useState([]);
    const [gekozenDatum, setGekozenDatum] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function load() {
            try {
                setLoading(true);
                const d = await fetchVeilingDagen(token);
                setDagen(d);
            } catch (e) {
                setError("Kon veildagen niet ophalen.");
            } finally {
                setLoading(false);
            }
        }

        load();
    }, [token]);

    async function handlePlan() {
        if (!gekozenDatum) {
            alert("Kies een veildatum.");
            return;
        }

        try {
            await startVeiling(token, gekozenDatum);
            alert("Veiling gestart!");
        } catch {
            alert("Veiling starten mislukt.");
        }
    }

    return (
        <main className="container py-4">
            <h1 className="h3 mb-3">Nieuwe Veiling Plannen</h1>

            {error && <div className="alert alert-danger">{error}</div>}

            {loading ? (
                <p>Laden…</p>
            ) : (
                <>
                    <label className="form-label">Beschikbare veildagen</label>

                    <select
                        className="form-select mb-3"
                        value={gekozenDatum}
                        onChange={(e) => setGekozenDatum(e.target.value)}
                    >
                        <option value="">-- Selecteer een dag --</option>
                        {dagen.map((d) => (
                            <option key={d} value={d}>
                                {d}
                            </option>
                        ))}
                    </select>

                    <button className="btn btn-success" onClick={handlePlan}>
                        Start veiling
                    </button>
                </>
            )}
        </main>
    );
}
