import { useEffect, useState } from "react";

const API_BASE = "https://localhost:56418";

export default function HeroBanner() {
    const [data, setData] = useState({
        titel: "Geen actieve veiling",
        beschrijving: "Er is momenteel geen veiling actief.",
        bid: "—",
        afbeelding: "/images/bloemen.jpg"
    });

    useEffect(() => {
        async function loadBanner() {
            try {
                const res = await fetch(
                    `${API_BASE}/api/veilingmeester/veilingen/actief`
                );

                if (!res.ok) {
                    // Geen actieve veiling → banner leeg laten
                    return;
                }

                const veiling = await res.json();

                if (!veiling || !veiling.huidigProduct) return;

                const product = veiling.huidigProduct;

                setData({
                    titel: product.soort,
                    beschrijving: `Hoeveelheid: ${product.hoeveelheid}`,
                    bid: `€${(product.huidigePrijs ?? product.startPrijs).toFixed(2)}`,
                    afbeelding: product.fotoUrl || "/images/bloemen.jpg"
                });
            } catch (err) {
                console.error("Kan banner niet laden:", err);
            }
        }

        loadBanner();
    }, []);

    return (
        <section className="hero-section py-5">
            <div className="container-fluid px-0">
                <div className="row g-0 align-items-center">
                    <div className="col-md-6 p-5 hero-text">
                        <h1 className="fw-bold mb-3">{data.titel}</h1>
                        <p className="text-muted mb-4">{data.beschrijving}</p>

                        <div className="bid-info d-flex align-items-center gap-3 mb-4">
                            <div className="last-bid-box">
                                <span className="label">Huidige prijs</span>
                                <span className="value">{data.bid}</span>
                            </div>
                        </div>

                        <a href="/actueelbod" className="btn" id="btn-herobanner">
                            Bekijk veiling
                        </a>
                    </div>

                    <div className="col-md-6 hero-image">
                        <img
                            src={data.afbeelding}
                            className="img-fluid w-100 h-100 object-fit-cover"
                            alt="Veiling banner"
                        />
                    </div>
                </div>
            </div>
        </section>
    );
}
