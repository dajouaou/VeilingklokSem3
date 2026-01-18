import { useEffect, useState, useContext } from "react";
import { AuthContext } from "../../auth/AuthContext";
import { fetchPlannedVeilingen } from "../../veiling/api/veilingApi";
import Sidebar from "./Sidebar";
import Topbar from "./Topbar";
import "../VeilingmeesterDashboard.css";

export default function GeplandeVeilingen() {

    // Token voor API-calls + logout functie uit context
    const { token, logout } = useContext(AuthContext);

    // State voor data en errors
    const [veilingen, setVeilingen] = useState([]);
    const [error, setError] = useState("");

    // UI-state voor sidebar (open/dicht)
    const [sidebarOpen, setSidebarOpen] = useState(false);

    useEffect(() => {
        // Bij mount of token wijziging: geplande veilingen ophalen
        fetchPlannedVeilingen(token)
        fetchPlannedVeilingen(token)
            .then(setVeilingen)
            .catch(() => setError("Kon geplande veilingen niet laden"));
    }, [token]);

    return (
        <div className="vm-shell">
            <Sidebar logout={logout} sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

            <div className="vm-content">
                <Topbar title="Geplande veilingen" veiling={null} onMenuClick={() => setSidebarOpen(true)} />

                <main className="vm-page">
                    {error && <div className="vm-alert vm-alert-danger">{error}</div>}

                    <section className="vm-hero-card">
                        <h2>Planning</h2>
                        <p className="vm-muted">Overzicht van alle geplande veilingen.</p>
                    </section>

                    {veilingen.length === 0 && (
                        <section className="vm-panel">
                            <p className="vm-muted">Geen geplande veilingen.</p>
                        </section>
                    )}

                    <section className="vm-grid-cards">
                        {veilingen.map((v) => (
                            <article key={v.id} className="vm-card">
                                <div className="vm-card-title">Veiling #{v.id}</div>
                                <div className="vm-muted vm-card-sub">
                                    Datum: {v.veildatum}
                                    <br />
                                    Start: {v.startTijd}
                                    <br />
                                    Producten: {v.aantalProducten}
                                </div>
                            </article>
                        ))}
                    </section>
                </main>
            </div>
        </div>
    );
}
