import { useEffect, useState } from "react";
import {
    getPublicActieveVeiling,
    getPublicVolgendeVeiling,
} from "../../features/veiling/api/veilingPublicApi";
import { Link } from "react-router-dom";

export default function HeroBanner() {
    const [data, setData] = useState({
        titel: "Geen actieve veiling",
        beschrijving: "Er is momenteel geen veiling actief.",
        bid: "—",
        afbeelding: "/images/bloemen.jpg",
        link: "/actueelbod",
    });

    useEffect(() => {
        async function load() {
            try {
                const actief = await getPublicActieveVeiling();

                if (actief?.huidigProduct) {
                    const p = actief.huidigProduct;
                    setData({
                        titel: p.soort,
                        beschrijving: `Resterend: ${p.resterendeHoeveelheid} stuks`,
                        bid: `€${(p.huidigePrijs ?? 0).toFixed(2)}`,
                        afbeelding: p.fotoUrl || "/images/bloemen.jpg",
                        link: "/actueelbod",
                    });
                    return;
                }

                const volgende = await getPublicVolgendeVeiling();
                if (volgende) {
                    setData({
                        titel: `Volgende veiling #${volgende.id}`,
                        beschrijving: `${volgende.veildatum} om ${volgende.startTijd} • ${volgende.aantalProducten} producten`,
                        bid: "—",
                        afbeelding: "/images/bloemen.jpg",
                        link: "/actueelbod",
                    });
                } else {
                    // optioneel: reset naar default als er echt niets is
                    setData((prev) => ({
                        ...prev,
                        titel: "Geen actieve veiling",
                        beschrijving: "Er is momenteel geen veiling actief.",
                        bid: "—",
                        afbeelding: "/images/bloemen.jpg",
                        link: "/actueelbod",
                    }));
                }
            } catch (err) {
                console.error("Kan banner niet laden:", err);
            }
        }

        load();
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

                        <Link to={data.link} className="btn" id="btn-herobanner">
                            Bekijk veiling
                        </Link>
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
