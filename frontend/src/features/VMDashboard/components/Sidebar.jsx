export function Sidebar({ activeKey, onNavigate }) {
    const items = [
        { key: "dashboard", label: "Dashboard" },
        { key: "active", label: "Actieve veiling" },
        { key: "audit", label: "Historie / audit" },
        { key: "settings", label: "Instellingen" },
    ];

    return (
        <aside className="vm-sidebar">
            <div className="vm-sidebar__brand">
                <div className="vm-brand__title">Veilingklok</div>
                <div className="vm-brand__subtitle">Veilingmeester</div>
            </div>

            <nav className="vm-sidebar__nav">
                {items.map((it) => (
                    <button
                        key={it.key}
                        className={`vm-nav__item ${activeKey === it.key ? "is-active" : ""}`}
                        onClick={() => onNavigate(it.key)}
                        type="button"
                    >
                        {it.label}
                    </button>
                ))}
            </nav>

            <div className="vm-sidebar__hint">
                <div className="vm-hint__title">Tip</div>
                <div className="vm-hint__text">Veilingplanning staat bewust niet in dit menu.</div>
            </div>
        </aside>
    );
}
