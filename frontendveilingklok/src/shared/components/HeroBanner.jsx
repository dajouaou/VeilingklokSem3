import { useEffect, useState } from "react";

const API_BASE = "https://localhost:56418";

export default function HeroBanner() {
    const [data, setData] = useState({
        titel: "Laden...",
        beschrijving: "Even geduld, veiling wordt geladen.",
        bid: "€0,00",
        afbeelding: "/images/bloemen.jpg"
    });

    useEffect(() => {
        async function loadBanner() {
            try {
                // 1. Actieve veilingen ophalen
                const r1 = await fetch(`${API_BASE}/api/veiling?status=Actief`);
                const veilingen = await r1.json();

                if (!veilingen.length) return;

                const v = veilingen[veilingen.length - 1];
                const veilingId = v.id ?? v.Id;

                // 2. Producten ophalen
                const r2 = await fetch(`${API_BASE}/api/veilingproduct/veiling/${veilingId}`);
                const koppelingen = await r2.json();
                const product = koppelingen.length ? koppelingen[0].product : null;

                // 3. Laatste bod ophalen
                const r3 = await fetch(`${API_BASE}/api/bod`);
                const biedingen = await r3.json();
                const biedingenVoorDeze = biedingen.filter(b => b.veilingId == veilingId);

                let prijs = 10;
                if (biedingenVoorDeze.length > 0) {
                    biedingenVoorDeze.sort((a, b) => b.bedrag - a.bedrag);
                    prijs = biedingenVoorDeze[0].bedrag;
                }

                // 4. State invullen
                setData({
                    titel: product?.naam ?? `Veiling #${veilingId}`,
                    beschrijving: product
                        ? `Aantal: ${product.hoeveelheid}`
                        : "Bekijk nu de nieuwste veiling.",
                    bid: `€${prijs.toFixed(2)}`,
                    afbeelding: product?.afbeeldingUrl ?? "/images/bloemen.jpg"
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

                        <div className="bid-info d-flex flex-wrap align-items-center gap-3 mb-4">
                            <div className="last-bid-box">
                                <span className="label">Laatste bod</span>
                                <span className="value">{data.bid}</span>
                            </div>
                        </div>

                        <a href="/actuelebod" className="btn" id="btn-herobanner">
                             Bekijk veiling
                        </a>
                    </div>

                    <div className="col-md-6 position-relative hero-image">
                        <img
                            src={data.afbeelding}
                            className="img-fluid w-100 h-100 object-fit-cover"
                            alt="Veiling banner"
                        />
                        <div className="curve-shape"></div>
                    </div>

                </div>
            </div>
        </section>
    );
}