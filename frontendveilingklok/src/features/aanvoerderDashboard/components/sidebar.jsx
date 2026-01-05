import React from "react";

export default function Sidebar({
    open,
    onClose,
    onNavigate,
    onLogout,
    userLabel = "Aanvoerder",
}) {
    return (
        <>
            {/* Overlay */}
            <div
                onClick={onClose}
                style={{
                    position: "fixed",
                    inset: 0,
                    background: open ? "rgba(0,0,0,0.35)" : "transparent",
                    pointerEvents: open ? "auto" : "none",
                    transition: "background 0.2s ease",
                    zIndex: 1040,
                }}
            />

            {/* Sidebar */}
            <aside
                aria-label="Zijmenu"
                style={{
                    position: "fixed",
                    top: 0,
                    left: 0,
                    height: "100vh",
                    width: 280,
                    background: "#fff",
                    borderRight: "1px solid #eee",
                    transform: open ? "translateX(0)" : "translateX(-100%)",
                    transition: "transform 0.2s ease",
                    zIndex: 1050,
                    display: "flex",
                    flexDirection: "column",
                }}
            >
                <div style={{ padding: "16px 16px 12px", borderBottom: "1px solid #eee" }}>
                    <div className="d-flex align-items-center justify-content-between">
                        <div>
                            <div className="fw-bold">Veilingklok</div>
                            <div className="text-muted" style={{ fontSize: 13 }}>
                                {userLabel}
                            </div>
                        </div>

                        <button
                            className="btn btn-sm btn-outline-secondary"
                            onClick={onClose}
                            aria-label="Sluiten"
                        >
                            ✕
                        </button>
                    </div>
                </div>

                <nav style={{ padding: 12, flex: 1, overflow: "auto" }}>
                    <div className="list-group">
                        <button
                            className="list-group-item list-group-item-action"
                            onClick={() => onNavigate("dashboard")}
                        >
                            Dashboard
                        </button>

                        <button
                            className="list-group-item list-group-item-action"
                            onClick={() => onNavigate("nieuw")}
                        >
                            Nieuw product aanmelden
                        </button>

                        <button
                            className="list-group-item list-group-item-action"
                            onClick={() => onNavigate("mijn")}
                        >
                            Mijn aanmeldingen
                        </button>

                        <button
                            className="list-group-item list-group-item-action"
                            onClick={() => onNavigate("beheer")}
                        >
                            Aanmeldingen beheren
                        </button>
                    </div>
                </nav>

                <div style={{ padding: 12, borderTop: "1px solid #eee" }}>
                    <button className="btn btn-danger w-100" onClick={onLogout}>
                        Uitloggen
                    </button>
                </div>
            </aside>
        </>
    );
}
