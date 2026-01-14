import { Link, useNavigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "../../features/auth/AuthContext";

export default function Navbar() {
    const { token, role, naam, logout } = useContext(AuthContext);
    const navigate = useNavigate();

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

    const isKoper = token && role === "Koper";
    const colors = ["#0d6efd", "#198754", "#6f42c1"];
    const color = colors[naam.length % colors.length];


    return (
        <nav className="navbar navbar-expand-lg bg-white sticky-top nav-elevated py-3">
            <div className="container">
                {/* LOGO */}
                <Link className="navbar-brand fw-bold" to="/">
                    <img
                        src="\src\Images\royal floraholland logo new.png"
                        alt="Veilingklok Logo"
                        className="header-logo"
                    />
                </Link>

                <div className="collapse navbar-collapse">
                    <ul className="navbar-nav ms-auto align-items-center gap-3">

                        <li className="nav-item">
                            <Link className="nav-link" to="/actueelbod">Actueel bod</Link>
                        </li>

                        {/* 👉 RECHTSBOVEN PROFIEL */}
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
                                    {/* cirkel met letter */}
                                    <div
                                        className="rounded-circle bg-dark text-white d-flex justify-content-center align-items-center"
                                        style={{ width: 32, height: 32, fontSize: 14 }}
                                    >
                                        {naam?.charAt(0)?.toUpperCase()}
                                    </div>

                                    <span className="fw-semibold">{naam}</span>
                                </button>

                                <ul className="dropdown-menu dropdown-menu-end shadow">
                                    <li>
                                        <span className="dropdown-item-text text-muted small">
                                            Ingelogd als
                                            <br />
                                            <strong>{naam}</strong>
                                        </span>
                                    </li>

                                    <li><hr className="dropdown-divider" /></li>

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
