import { useEffect, useState } from "react";

const API_BASE = "https://localhost:56418";

export default function NextBids() {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {

        async function loadNext() {
            try {
                // 1. Actieve veiling
                const r1 = await fetch(`${API_BASE}/api/veiling?status=Actief`);
                const veilingen = await r1.json();

                if (!veilingen.length) {
                    setLoading(false);
                    return;
                }

                const v = veilingen[veilingen.length - 1];
                const veilingId = v.id ?? v.Id;

                // 2. Producten voor veiling
                const r2 = await fetch(`${API_BASE}/api/veilingproduct/veiling/${veilingId}`);
                const koppelingen = await r2.json();

                setProducts(koppelingen);
                setLoading(false);

            } catch (err) {
                console.error("Kan volgende biedingen niet laden:", err);
                setLoading(false);
            }
        }

        loadNext();
    }, []);

    return (
        <section id="gerelateerd" className="py-5 bg-light">
            <div className="container">
                <h2 className="fw-bold mb-4">Volgende Biedingen (Live)</h2>

                <div className="row g-4">
                    {loading && <p className="text-muted">Laden...</p>}

                    {!loading && products.length === 0 && (
                        <p className="text-muted">Geen producten gekoppeld aan deze veiling.</p>
                    )}

                    {products.map(koppeling => {
                        const p = koppeling.product;

                        return (
                            <div className="col-sm-6 col-md-4 col-lg-3" key={p.id}>
                                <div className="card card-product h-100">
                                    <img
                                        src={p.afbeeldingUrl ?? '/images/bloemen.jpg'}
                                        className="card-img-top"
                                        alt={p.naam}
                                    />

                                    <div className="card-body">
                                        <h5 className="card-title">{p.naam}</h5>
                                        <p className="card-text">Aantal: {p.hoeveelheid}</p>

                                        <span className="text-success fw-semibold">
                                            €{(p.startPrijs ?? 10).toFixed(2)}
                                        </span>

                                        <div className="d-flex justify-content-start mt-2">
                                            <a href="/actuelebod" className="btn btn-sm btn-view-product">
                                                Bekijk in veiling
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        );
                    })}
                </div>

            </div>
        </section>
    );
}
