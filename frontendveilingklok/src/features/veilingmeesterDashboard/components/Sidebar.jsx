import { NavLink, useNavigate } from "react-router-dom";
import { iconUrl } from "../../../shared/icons/iconUrl.js";

export default function Sidebar({ logout, sidebarOpen, setSidebarOpen }) {
    const navigate = useNavigate();

    function handleLogout() {
        // Logout verwijdert token/role uit context
        logout();

        // Daarna redirect naar login, replace voorkomt "terug-knop" naar protected pagina
        navigate("/login", { replace: true });
    }

    return (
        <aside className={`vm-sidebar ${sidebarOpen ? "open" : ""}`} aria-label="Navigatie">
            {/* Sluit-knop: zet sidebar state dicht */}
            <button
                type="button"
                className="vm-sidebar-close"
                onClick={() => setSidebarOpen(false)}
                aria-label="Sluit menu"
            >
                Sluiten
            </button>

            <div className="vm-sidebar-brand">
                <div className="vm-brand-dot" />
                <div>
                    <div className="vm-brand-title">Veilingklok</div>
                    <div className="vm-brand-sub">Veilingmeester</div>
                </div>
            </div>

            <nav className="vm-nav">
                {/* NavLink: actief linkje krijgt automatisch "active" class van react-router */}
                <NavLink to="/veilingmeester" end onClick={() => setSidebarOpen(false)}>
                    <img src={iconUrl("nav/dashboard.svg")} alt="" width="18" height="18" />
                    <span>Dashboard</span>
                </NavLink>

                <NavLink to="/veilingmeester/plan" onClick={() => setSidebarOpen(false)}>
                    <img src={iconUrl("nav/plan.svg")} alt="" width="18" height="18" />
                    <span>Veiling plannen</span>
                </NavLink>

                <NavLink to="/veilingmeester/gepland" onClick={() => setSidebarOpen(false)}>
                    <img src={iconUrl("nav/calendar.svg")} alt="" width="18" height="18" />
                    <span>Geplande veilingen</span>
                </NavLink>

                <NavLink to="/veilingmeester/archief" onClick={() => setSidebarOpen(false)}>
                    <img src={iconUrl("nav/archive.svg")} alt="" width="18" height="18" />
                    <span>Archief</span>
                </NavLink>
            </nav>

            <div className="vm-sidebar-footer">
                <button type="button" className="vm-logout" onClick={handleLogout}>
                    <img src={iconUrl("ui/logout.svg")} alt="" width="18" height="18" />
                    Uitloggen
                </button>
            </div>
        </aside>
    );
}
