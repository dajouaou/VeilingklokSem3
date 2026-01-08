import { Link, useLocation, useNavigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "../../auth/AuthContext";

export default function AanvoerderNavbar() {
    const { logout } = useContext(AuthContext);
    const navigate = useNavigate();
    const { pathname } = useLocation();

    const isActive = (p) => pathname === p;

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

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
                            <Link className={`nav-link ${isActive("/aanvoerder") ? "active" : ""}`} to="/aanvoerder">
                                Dashboard
                            </Link>
                        </li>

                        <li className="nav-item">
                            <Link className="nav-link" to="/">
                                Home (koper view)
                            </Link>
                        </li>

                        <li className="nav-item ms-lg-3">
                            <button className="btn btn-outline-danger rounded-pill px-3" onClick={handleLogout}>
                                Uitloggen
                            </button>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    );
}
