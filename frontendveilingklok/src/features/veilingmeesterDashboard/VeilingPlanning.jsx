import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../auth/AuthContext.jsx";
import Sidebar from "./components/Sidebar.jsx";
import { fetchAanvoerderProducten, startVeiling } from "./api/veilingApi.js";

export default function VeilingPlanning() {
    const { token } = useContext(AuthContext);

    const [producten, setProducten] = useState([]);
    const [loading, setLoading] = useState(true);

    const [productId, setProductId] = useState("");
    const [veildatum, setVeildatum] = useState("");
    const [startTijd, setStartTijd] = useState("09:00");
    const [eindTijd, setEindTijd] = useState("10:00");

    const [message, setMessage] = useState("");

    useEffect(() => {
        async function load() {
            try {
                const p = await fetchAanvoerderProducten();
                setProducten(p);
            } catch {
                setMessage("Kon producten niet laden.");
            } finally {
                setLoading(false);
            }
        }
        load();
    }, []);

    async function handleStart() {
        setMessage("");

        if (!productId || !veildatum) {
            setMessage("Kies product en datum.");
            return;
        }

        try {
            await startVeiling({
                token,
                productId,
                veildatum,
                startTijd,
                eindTijd
            });

            setMessage("Veiling succesvol gestart!");
        } catch (err) {
            setMessage("Starten mislukt.");
        }
    }

    return (
        <div className="vm-layout">
            <Sidebar active="planning" />

            <main className="vm-main">
                <div className="vm-topbar">
                    <h1>Veiling plannen</h1>
                </div>

                <div className="vm-main-content">
                    <section className="vm-card vm-card-highlight">
                        <h2>Nieuwe veiling starten</h2>

                        {message && <div className="alert alert-info">{message}</div>}
                        {loading && <p>Laden...</p>}

                        {!loading && (
                            <>
                                <label>Product *</label>
                                <select
                                    className="form-select"
                                    value={productId}
                                    onChange={(e) => setProductId(e.target.value)}
                                >
                                    <option value="">-- Selecteer product --</option>
                                    {producten.map(p => (
                                        <option key={p.id} value={p.id}>
                                            {p.soort} - {p.hoeveelheid} st. - {p.aanvoerderNaam}
                                        </option>
                                    ))}
                                </select>

                                <label className="mt-3">Veildatum *</label>
                                <input
                                    type="date"
                                    className="form-control"
                                    value={veildatum}
                                    onChange={(e) => setVeildatum(e.target.value)}
                                />

                                <div className="d-flex gap-3 mt-3">
                                    <div>
                                        <label>Starttijd *</label>
                                        <input
                                            type="time"
                                            className="form-control"
                                            value={startTijd}
                                            onChange={(e) => setStartTijd(e.target.value)}
                                        />
                                    </div>

                                    <div>
                                        <label>Eindtijd *</label>
                                        <input
                                            type="time"
                                            className="form-control"
                                            value={eindTijd}
                                            onChange={(e) => setEindTijd(e.target.value)}
                                        />
                                    </div>
                                </div>

                                <button
                                    className="btn btn-primary mt-4"
                                    onClick={handleStart}
                                >
                                    Veiling starten
                                </button>
                            </>
                        )}
                    </section>
                </div>
            </main>
        </div>
    );
}
