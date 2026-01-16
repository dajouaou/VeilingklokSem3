import { Link } from "react-router-dom";

export default function CTASection() {
    return (
        <section className="cta-section py-5">
            <div className="container">
                <div className="cta-card">
                    <div className="row g-4 align-items-center">
                        <div className="col-md-8">
                            <h3 className="fw-bold mb-2">Klaar om live te bieden?</h3>
                            <p className="text-muted mb-0">
                                Ga naar het 'Actueel Bod'-pagina om te bieden.
                            </p>
                        </div>

                        <div className="col-md-4 d-flex gap-2 justify-content-md-end">
                            <Link to="/actueelbod" className="btn btn-outline-dark rounded-pill px-4">
                                Actueel Bod
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}