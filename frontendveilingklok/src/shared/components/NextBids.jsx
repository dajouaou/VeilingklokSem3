import React from "react";

export default function NextBids() {

    return (
        <section id="gerelateerd" className="py-5 bg-light">
            <div className="container">
                <h2 className="fw-bold mb-4">Volgende Biedingen</h2>

                <p className="text-muted">
                    (Veilingfunctionaliteit is tijdelijk uitgeschakeld.)
                </p>

                <div className="row g-4">

                    <div className="col-sm-6 col-md-4 col-lg-3">
                        <div className="card card-product h-100">
                            <img
                                src="/images/bloemen.jpg"
                                className="card-img-top"
                                alt="placeholder"
                            />
                            <div className="card-body">
                                <h5 className="card-title">Placeholder product</h5>
                                <p className="card-text">Aantal: 0</p>

                                <span className="text-success fw-semibold">
                                    €0.00
                                </span>

                                <div className="d-flex justify-content-start mt-2">
                                    <a className="btn btn-sm btn-view-product disabled">
                                        Bekijk in veiling
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

            </div>
        </section>
    );
}
