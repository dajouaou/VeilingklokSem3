import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../auth/AuthContext";
import Sidebar from "./components/Sidebar";
import Topbar from "./components/Topbar";
import { fetchArchiefVeilingen } from "../veiling/api/veilingApi";
import "./VeilingmeesterDashboard.css";

export default function Archief() {
    const { token, logout } = useContext(AuthContext);
    const [sidebarOpen, setSidebarOpen] = useState(false);

    const [items, setItems] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!token) return;

        fetchArchiefVeilingen(token)
            .then((data) => setItems(data ?? []))
            .catch((e) => setError(e.message || "Kon archief niet laden"));
    }, [token]);

    return (
        <div className="vm-layout">
            <Sidebar logout={logout} sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

            <div className="vm-main">
                <Topbar title="Archief" veiling={null} onMenuClick={() => setSidebarOpen(true)} />

                <main className="vm-main-content">
                    {error && <div className="alert alert-danger vm-alert">{error}</div>}

                    <section className="vm-card vm-card-highlight mb-4">
                        <h2>Afgelopen veilingen</h2>
                        <p className="text-muted mb-0">Overzicht van afgesloten veilingen inclusief koper, tijdstip en prijs.</p>
                    </section>

                    {items.length === 0 && (
                        <div className="vm-card">
                            <p className="text-muted mb-0">Nog geen afgesloten veilingen.</p>
                        </div>
                    )}

                    {items.map((v) => (
                        <div key={v.id} className="vm-card mb-3">
                            <div className="d-flex justify-content-between align-items-start">
                                <div>
                                    <h5 className="mb-1">Veiling #{v.id}</h5>
                                    <div className="text-muted small">
                                        Veildatum: {v.veildatum} · Start: {v.startTijd} · Einde: {v.eindTijd}
                                    </div>
                                </div>
                                <div className="badge bg-secondary">{v.aantalProducten} producten</div>
                            </div>

                            <hr />

                            {!v.transacties?.length ? (
                                <p className="text-muted mb-0">Geen transacties gevonden.</p>
                            ) : (
                                <div className="table-responsive">
                                    <table className="table table-sm align-middle mb-0">
                                        <thead>
                                            <tr>
                                                <th>Product</th>
                                                <th>Koper</th>
                                                <th>Aantal</th>
                                                <th>Prijs</th>
                                                <th>Tijd</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {v.transacties.map((t, idx) => (
                                                <tr key={idx}>
                                                    <td>{t.soort}</td>
                                                    <td>{t.koperNaam}</td>
                                                    <td>{t.aantal}</td>
                                                    <td>{t.prijs?.toFixed(2)} EUR</td>
                                                    <td>{t.tijdstip ? new Date(t.tijdstip).toLocaleString("nl-NL") : "-"}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            )}
                        </div>
                    ))}
                </main>
            </div>
        </div>
    );
}
