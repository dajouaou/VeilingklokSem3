import { Link, useLocation, useNavigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "../../auth/AuthContext";

export default function AanvoerderNavbar() {
    const { logout, naam, role, token } = useContext(AuthContext);
    const navigate = useNavigate();
    const { pathname } = useLocation();

    const isActive = (p) => pathname === p;

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

    const isAanvoerder = token && role === "Aanvoerder";

    return (
        <nav className="navbar navbar-expand-lg bg-white border-bottom sticky-top">
            <div className="container">
                <Link className="navbar-brand fw-bold" to="/aanvoerder">
                    Aanvoerder
                </Link>

                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#aanvoerderNav"
                    aria-controls="aanvoerderNav"
                    aria-expanded="false"
                    aria-label="Menu"
                >
                    <span className="navbar-toggler-icon" />
                </button>

                <div id="aanvoerderNav" className="collapse navbar-collapse">
                    <ul className="navbar-nav ms-auto align-items-lg-center gap-lg-2">
                        <li className="nav-item">
                            <Link
                                className={`nav-link ${isActive("/aanvoerder") ? "active" : ""}`}
                                to="/aanvoerder"
                            >
                                Dashboard
                            </Link>
                        </li>

                        {/* PROFIEL RECHTSBOVEN */}
                        {isAanvoerder && (
                            <li className="nav-item dropdown ms-lg-3">
                                <button
                                    className="btn btn-light dropdown-toggle d-flex align-items-center gap-2"
                                    data-bs-toggle="dropdown"
                                    aria-expanded="false"
                                >
                                    <div
                                        className="rounded-circle bg-dark text-white d-flex justify-content-center align-items-center"
                                        style={{ width: 32, height: 32, fontSize: 14 }}
                                    >
                                        {naam?.charAt(0)?.toUpperCase() || "A"}
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
                                    <li>
                                        <hr className="dropdown-divider" />
                                    </li>
                                    <li>
                                        <button
                                            className="dropdown-item text-danger"
                                            onClick={handleLogout}
                                        >
                                            Uitloggen
                                        </button>
                                    </li>
                                </ul>
                            </li>
                        )}
                    </ul>
                </div>
            </div>
        </nav>
    );
}
