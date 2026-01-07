import { NavLink, useNavigate } from "react-router-dom";
import { iconUrl } from "../../../shared/icons/iconUrl.js";

export default function Sidebar({ logout, sidebarOpen, setSidebarOpen }) {
    const navigate = useNavigate();

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

    return (
        <aside className={`vm-sidebar ${sidebarOpen ? "open" : ""}`}>
            <button className="vm-sidebar-close" onClick={() => setSidebarOpen(false)} aria-label="Sluit menu">
                ✖
            </button>

            <div className="vm-sidebar-logo">
                <div className="vm-logo-dot" />
                <div>
                    <div className="vm-logo-title">Veilingklok</div>
                    <div className="vm-logo-sub">Veilingmeester</div>
                </div>
            </div>

            <nav className="vm-sidebar-nav">
                <NavLink to="/veilingmeester" end onClick={() => setSidebarOpen(false)}>
                    {({ isActive }) => (
                        <>
                            <img src={iconUrl("nav/dashboard.svg")} alt="" width="18" height="18" />
                            <span className={isActive ? "active" : ""}>Dashboard</span>
                        </>
                    )}
                </NavLink>

                <NavLink to="/veilingmeester/plan" onClick={() => setSidebarOpen(false)}>
                    <img src={iconUrl("nav/plan.svg")} alt="" width="18" height="18" />
                    Veiling plannen
                </NavLink>

                <NavLink to="/veilingmeester/gepland" onClick={() => setSidebarOpen(false)}>
                    <img src={iconUrl("nav/calendar.svg")} alt="" width="18" height="18" />
                    Geplande veilingen
                </NavLink>

                <NavLink to="/veilingmeester/archief" onClick={() => setSidebarOpen(false)}>
                    <img src={iconUrl("nav/archive.svg")} alt="" width="18" height="18" />
                    Archief
                </NavLink>
            </nav>

            <div className="vm-sidebar-footer">
                <button className="vm-logout-btn" onClick={handleLogout}>
                    <img
                        src={iconUrl("ui/logout.svg")}
                        alt=""
                        width="18"
                        height="18"
                        style={{ marginRight: 8, verticalAlign: "middle" }}
                    />
                    Uitloggen
                </button>
            </div>
        </aside>
    );
}
