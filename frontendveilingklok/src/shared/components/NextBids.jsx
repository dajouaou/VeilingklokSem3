import { useEffect, useState } from "react";
import { getPublicVolgendeVeiling } from "../../features/veiling/api/veilingPublicApi";
import flowerbanner1 from "../Images/flowerbanner1.jpg";


export default function NextBids() {
    const [volgende, setVolgende] = useState(null);

    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const v = await getPublicVolgendeVeiling();
                if (!alive) return;
                setVolgende(v || null);
            } catch {
                if (!alive) return;
                setVolgende(null);
            }
        }

        load();
        const interval = setInterval(load, 10000); // elke 10 sec

        return () => {
            alive = false;
            clearInterval(interval);
        };
    }, []);

    return (
        <section id="gerelateerd" className="py-5 bg-light">
            <div className="container">
                <h2 className="fw-bold mb-2">Volgende veiling</h2>
                <p className="text-muted mb-4">Overzicht van de eerstvolgende geplande veiling.</p>

                {!volgende && (
                    <div className="card p-4">
                        <div className="text-muted">Geen geplande veiling gevonden.</div>
                    </div>
                )}

                {volgende && (
                    <div className="row g-4">
                        <div className="col-md-6 col-lg-5">
                            <div className="card card-product h-100">
                                <img src={flowerbanner1} className="card-img-top" alt="Veiling" />
                                <div className="card-body">
                                    <h5 className="card-title">Veiling #{volgende.id}</h5>
                                    <p className="card-text mb-1">
                                        Datum: <b>{volgende.veildatum}</b>
                                    </p>
                                    <p className="card-text mb-1">
                                        Start: <b>{volgende.startTijd}</b>
                                    </p>
                                    <p className="card-text mb-3">
                                        Producten: <b>{volgende.aantalProducten}</b>
                                    </p>
                                    <span className="text-success fw-semibold">Status: gepland</span>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-6 col-lg-7">
                            <div className="card p-4 h-100">
                                <h5 className="fw-bold mb-2">Tip</h5>
                                <p className="text-muted mb-0">
                                    Ga naar <b>Actueel bod</b> om live mee te doen zodra de veiling start.
                                </p>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </section>
    );
}
