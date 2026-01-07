import { useEffect, useState, useContext } from "react";
import { AuthContext } from "../../auth/AuthContext";
import { fetchPlannedVeilingen } from "../../veiling/api/veilingApi";
import Sidebar from "./Sidebar";
import Topbar from "./Topbar";
import "../VeilingmeesterDashboard.css";

export default function GeplandeVeilingen() {
    const { token, logout } = useContext(AuthContext);

    const [veilingen, setVeilingen] = useState([]);
    const [error, setError] = useState("");
    const [sidebarOpen, setSidebarOpen] = useState(false);

    useEffect(() => {
        fetchPlannedVeilingen(token)
            .then(setVeilingen)
            .catch(() => setError("Kon geplande veilingen niet laden"));
    }, [token]);

    return (
        <div className="vm-layout">
            <Sidebar logout={logout} sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

            <div className="vm-main">
                <Topbar title="Geplande veilingen" veiling={null} onMenuClick={() => setSidebarOpen(true)} />

                <main className="vm-main-content">
                    {error && <div className="alert alert-danger vm-alert">{error}</div>}

                    {veilingen.length === 0 && (
                        <div className="vm-card">
                            <p className="text-muted mb-0">Geen geplande veilingen.</p>
                        </div>
                    )}

                    {veilingen.map((v) => (
                        <div key={v.id} className="vm-card mb-3">
                            <strong>Veiling #{v.id}</strong>
                            <div className="text-muted small mt-1">
                                Datum: {v.veildatum} · Starttijd: {v.startTijd} · Producten: {v.aantalProducten}
                            </div>
                        </div>
                    ))}
                </main>
            </div>
        </div>
    );
}
