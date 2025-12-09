import { Link } from "react-router-dom";

export default function Navbar() {
    return (
        <nav className="navbar navbar-expand-lg bg-white sticky-top nav-elevated py-3">
            <div className="container">

                {/* LOGO */}
                <Link className="navbar-brand d-flex align-items-center gap-2 fw-bold" to="/">
                    <img src="/images/royal floraholland logo new.png" alt="Veilingklok Logo" className="header-logo" />
                </Link>

                <button className="navbar-toggler shadow-none border-0" type="button"
                    data-bs-toggle="collapse" data-bs-target="#mainNav"
                    aria-controls="mainNav" aria-expanded="false" aria-label="Menu">
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div id="mainNav" className="collapse navbar-collapse">
                    <ul className="navbar-nav ms-auto align-items-lg-center gap-lg-2">

                        {/* Actueel bod */}
                        <li className="nav-item">
                            <Link className="nav-link" to="/actueelbod">
                                Actueel bod
                            </Link>
                        </li>

                        {/* Veiling */}
                        <li className="nav-item">
                            <Link className="nav-link" to="/veiling">
                                Veiling
                            </Link>
                        </li>

                        {/* Login */}
                        <li className="nav-item">
                            <Link className="btn btn-dark rounded-pill px-3" to="/login">
                                <i className="bi bi-box-arrow-in-right me-1"></i> Login
                            </Link>
                        </li>

                    </ul>
                </div>
            </div>
        </nav>
    );
}
