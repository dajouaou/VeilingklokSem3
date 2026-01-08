import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../../auth/AuthContext"; // pas pad aan indien nodig

export default function Topbar({ title, veiling, onMenuClick }) {
    const { logout, naam, role, token } = useContext(AuthContext);
    const navigate = useNavigate();

    const isVeilingmeester = token && role === "Veilingmeester";

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

    return (
        <header className="vm-topbar">
            <button className="vm-hamburger" onClick={onMenuClick}>
                Menu
            </button>

            <div>
                <h1>{title}</h1>
                <p>Beheer de klok, wachtrij en biedingen.</p>
            </div>

            <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
                {veiling && (
                    <div className="vm-topbar-pill">
                        <span className="dot" />
                        Veiling #{veiling.id}
                    </div>
                )}

                {isVeilingmeester && (
                    <div className="dropdown">
                        <button
                            className="btn btn-light dropdown-toggle d-flex align-items-center gap-2"
                            data-bs-toggle="dropdown"
                            aria-expanded="false"
                        >
                            <div
                                className="rounded-circle bg-dark text-white d-flex justify-content-center align-items-center"
                                style={{ width: 32, height: 32, fontSize: 14 }}
                            >
                                {naam?.charAt(0)?.toUpperCase() || "V"}
                            </div>
                            <span className="fw-semibold">{naam}</span>
                        </button>

                        <ul className="dropdown-menu dropdown-menu-end shadow">
                            <li>
                                <span className="dropdown-item-text text-muted small">
                                    Ingelogd als<br />
                                    <strong>{naam}</strong>
                                </span>
                            </li>
                            <li><hr className="dropdown-divider" /></li>
                            <li>
                                <button className="dropdown-item text-danger" onClick={handleLogout}>
                                    Uitloggen
                                </button>
                            </li>
                        </ul>
                    </div>
                )}
            </div>
        </header>
    );
}
