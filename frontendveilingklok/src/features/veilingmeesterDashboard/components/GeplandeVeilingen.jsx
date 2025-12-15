import { useEffect, useState, useContext } from "react";
import { AuthContext } from "../../auth/AuthContext";
import { fetchPlannedVeilingen } from "../../veiling/api/veilingApi";
import Sidebar from "./Sidebar";


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
            <Sidebar
                logout={logout}
                active="gepland"
                sidebarOpen={sidebarOpen}
                setSidebarOpen={setSidebarOpen}
            />

            <div className="vm-main">
                <header className="vm-topbar">
                    <button
                        className="vm-hamburger"
                        onClick={() => setSidebarOpen(true)}
                    >
                        ? Menu
                    </button>

                    <div>
                        <h1>Geplande veilingen</h1>
                        <p>Kies welke veiling als volgende gestart wordt</p>
                    </div>
                </header>

                <main className="vm-main-content">
                    {error && <div className="alert alert-danger">{error}</div>}

                    {veilingen.length === 0 && (
                        <p className="text-muted">Geen geplande veilingen.</p>
                    )}

                    {veilingen.map(v => (
                        <div key={v.id} className="vm-card mb-3">
                            <strong>Veiling #{v.id}</strong><br />
                            Datum: {v.veildatum}<br />
                            Starttijd: {v.startTijd}<br />
                            Producten: {v.aantalProducten}
                        </div>
                    ))}
                </main>
            </div>
        </div>
    );
}
