import { Link, useNavigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "../../features/auth/AuthContext";
import logo from "../../Images/royal floraholland logo new.png";

export default function Navbar() {
    const { token, role, naam, logout } = useContext(AuthContext);
    const navigate = useNavigate();

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

    const isKoper = !!token && role === "Koper";

    // FIX: naam kan null/undefined zijn
    const safeNaam = naam ?? "";

    const colors = ["#0d6efd", "#198754", "#6f42c1"];
    const color = colors[safeNaam.length % colors.length];

    return (
        <nav className="navbar navbar-expand-lg bg-white sticky-top nav-elevated py-3">
            <div className="container">
                <Link className="navbar-brand fw-bold" to="/">
                    <img
                        src={logo}
                        alt="Veilingklok Logo"
                        className="header-logo"
                    />
                </Link>

                <div className="collapse navbar-collapse">
                    <ul className="navbar-nav ms-auto align-items-center gap-3">
                        <li className="nav-item">
                            <Link className="nav-link" to="/actueelbod">Actueel bod</Link>
                        </li>

                        {!isKoper ? (
                            <li className="nav-item">
                                <Link className="btn btn-dark rounded-pill px-3" to="/login">
                                    Login
                                </Link>
                            </li>
                        ) : (
                            <li className="nav-item dropdown">
                                <button
                                    className="btn btn-light dropdown-toggle d-flex align-items-center gap-2"
                                    data-bs-toggle="dropdown"
                                    aria-expanded="false"
                                >
                                    <div
                                        className="rounded-circle text-white d-flex justify-content-center align-items-center"
                                        style={{ width: 32, height: 32, fontSize: 14, background: color }}
                                    >
                                        {(safeNaam.charAt(0) || "K").toUpperCase()}
                                    </div>

                                    <span className="fw-semibold">{safeNaam}</span>
                                </button>

                                <ul className="dropdown-menu dropdown-menu-end shadow">
                                    <li>
                                        <span className="dropdown-item-text text-muted small">
                                            Ingelogd als<br />
                                            <strong>{safeNaam}</strong>
                                        </span>
                                    </li>

                                    <li><hr className="dropdown-divider" /></li>

                                    <li>
                                        <button className="dropdown-item text-danger" onClick={handleLogout}>
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
