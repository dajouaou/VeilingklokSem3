export default function Navbar() {
    return (
        <nav className="navbar navbar-expand-lg bg-white sticky-top nav-elevated py-3">
            <div className="container">
                <a className="navbar-brand d-flex align-items-center gap-2 fw-bold" href="/">
                    <img src="/images/royal floraholland logo new.png" alt="Veilingklok Logo" className="header-logo" />
                </a>

                <button className="navbar-toggler shadow-none border-0" type="button"
                    data-bs-toggle="collapse" data-bs-target="#mainNav"
                    aria-controls="mainNav" aria-expanded="false" aria-label="Menu">
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div id="mainNav" className="collapse navbar-collapse">
                    <ul className="navbar-nav ms-auto align-items-lg-center gap-lg-2">

                        <li className="nav-item">
                            <a className="nav-link" href="/actuelebod">Actuele bod</a>
                        </li>

                        <li className="nav-item">
                            <a className="nav-link" href="/veiling">Veiling</a>
                        </li>

                        <li className="nav-item">
                            <a className="btn btn-dark rounded-pill px-3" href="/login">
                                <i className="bi bi-box-arrow-in-right me-1"></i> Login
                            </a>
                        </li>

                    </ul>
                </div>
            </div>
        </nav>
    );
}
