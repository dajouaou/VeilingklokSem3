import { Link } from "react-router-dom";

export default function CTASection({ isLoggedIn, isKoper }) {
    return (
        <section className="cta-section py-5">
            <div className="container">
                <div className="cta-card">
                    <div className="row align-items-center">
                        <div className="col-md-8">
                            <h3>Klaar om live te bieden?</h3>

                            <p>
                                {isKoper
                                    ? "Ga naar Actueel Bod om mee te bieden."
                                    : "Log in of registreer om mee te doen."}
                            </p>
                        </div>

                        <div className="col-md-4 d-flex gap-2 justify-content-md-end">
                            {!isLoggedIn && (
                                <>
                                    <Link to="/login" className="btn btn-dark">
                                        Inloggen
                                    </Link>
                                    <Link to="/register" className="btn btn-outline-dark">
                                        Registreren
                                    </Link>
                                </>
                            )}

                            {isKoper && (
                                <Link to="/actueelbod" className="btn btn-dark">
                                    Naar Actueel Bod
                                </Link>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}
