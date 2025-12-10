export default function Sidebar({ logout, active, sidebarOpen, setSidebarOpen }) {
    return (
        <aside className={`vm-sidebar ${sidebarOpen ? "open" : ""}`}>
            <button className="vm-sidebar-close" onClick={() => setSidebarOpen(false)}>
                ?
            </button>

            <div className="vm-sidebar-logo">
                <div className="vm-logo-dot" />
                <div>
                    <div className="vm-logo-title">Veilingklok</div>
                    <div className="vm-logo-sub">Veilingmeester</div>
                </div>
            </div>

            <nav className="vm-sidebar-nav">
                <a className={active === "dashboard" ? "active" : ""} href="/veilingmeester">
                    <span className="vm-nav-dot" />
                    Dashboard
                </a>
                <a className={active === "planning" ? "active" : ""} href="/veilingmeester/plan">
                    Veiling plannen
                </a>
                <a className={active === "statistiek" ? "active" : ""} href="/veilingmeester/statistieken">
                    <span className="vm-nav-dot" />
                    Statistieken
                </a>
            </nav>

            <div className="vm-sidebar-footer">
                <button className="vm-logout-btn" onClick={logout}>Uitloggen</button>
            </div>
        </aside>
    );
}
