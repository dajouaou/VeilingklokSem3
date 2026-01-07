import logo from "../../Images/royal floraholland logo new.png";

export default function Footer() {
    return (
        <footer className="footer mt-5">
            <div className="container-fluid px-0">
                <div className="row g-0 align-items-center position-relative">

                    {/* Donker groen */}
                    <div className="col-lg-7 p-5 footer-dark">
                        <div className="row">
                            <div className="col-md-4 mb-4 mb-md-0">
                                <img
                                    src={logo}
                                    alt="Royal FloraHolland"
                                    className="footer-logo mb-3"
                                />
                            </div>

                            <div className="col-md-4 mb-3">
                                <ul className="list-unstyled">
                                    <li><a href="#">Storingen/gepland onderhoud</a></li>
                                    <li><a href="#">Royal FloraHolland apps</a></li>
                                    <li><a href="#">Pers en Media</a></li>
                                    <li><a href="#">Aanmelden nieuwsbrief</a></li>
                                    <li><a href="#">Intranet Royal FloraHolland</a></li>
                                </ul>
                            </div>

                            <div className="col-md-4 mb-3">
                                <ul className="list-unstyled">
                                    <li><a href="#">Service en contact</a></li>
                                    <li><a href="#">Hulp op afstand</a></li>
                                    <li><a href="#">WhatsApp</a></li>
                                    <li><a href="#">Contactformulier</a></li>
                                    <li><a href="#">Locaties</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    {/* Lichtgroen */}
                    <div className="col-lg-5 p-5 footer-light position-relative">
                        <div className="curve-footer"></div>
                        <div className="d-flex justify-content-end align-items-center gap-3 social-icons pe-4">
                            <a href="#"><i className="bi bi-linkedin"></i></a>
                            <a href="#"><i className="bi bi-youtube"></i></a>
                            <a href="#"><i className="bi bi-facebook"></i></a>
                            <a href="#"><i className="bi bi-instagram"></i></a>
                        </div>
                    </div>
                </div>

                <div className="footer-bottom py-3 border-top text-center text-md-start px-5">
                    <small className="text-muted">
                        © 2025 Royal FloraHolland —
                        <a href="#"> Privacyverklaring</a> |
                        <a href="#"> Cookieverklaring</a> |
                        <a href="#"> CVD</a> |
                        <a href="#"> Algemene voorwaarden</a>
                    </small>
                </div>
            </div>
        </footer>
    );
}