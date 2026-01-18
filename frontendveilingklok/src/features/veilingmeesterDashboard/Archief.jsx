import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../auth/AuthContext";
import Sidebar from "./components/Sidebar";
import Topbar from "./components/Topbar";
import { fetchArchiefVeilingen } from "../veiling/api/veilingApi";
import "./VeilingmeesterDashboard.css";

export default function Archief() {
    const { token, logout } = useContext(AuthContext);
    const [sidebarOpen, setSidebarOpen] = useState(false);

    // Archief data + foutmelding
    const [items, setItems] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        // Zonder token geen call doen (beschermt tegen 401 spam)
        if (!token) return;

        fetchArchiefVeilingen(token)
            // data kan null zijn dan fallback []
            .then((data) => setItems(data ?? []))
            .catch((e) => setError(e.message || "Kon archief niet laden"));
    }, [token]);

    return (
        <div className="vm-shell">
            <Sidebar logout={logout} sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

            <div className="vm-content">
                <Topbar title="Archief" veiling={null} onMenuClick={() => setSidebarOpen(true)} />

                <main className="vm-page vm-archief-wrap">
                    {/* Foutmelding uit API netjes tonen */}
                    {error && <div className="vm-alert vm-alert-danger">{error}</div>}

                    {/* rest is UI, maar logica: conditionals op items/transacties */}
                </main>
            </div>
        </div>
    );
}
