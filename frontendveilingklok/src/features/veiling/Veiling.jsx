import { useEffect, useState } from "react";
import Navbar from "../../shared/components/Navbar";
import Footer from "../../shared/components/Footer";
import { getBeeindigdeBiedingen } from "./api/veilingPublicApi";

export default function Veiling() {
    const [biedingen, setBiedingen] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const data = await getBeeindigdeBiedingen();
                if (!alive) return;
                setBiedingen(data || []);
            } catch (e) {
                console.error("Fout bij laden biedingen:", e);
                if (alive) setBiedingen([]);
            } finally {
                if (alive) setLoading(false);
            }
        }

        load();

        return () => { alive = false; };
    }, []);

    return (
        <>
            <Navbar />
            <div className="container py-5">
                <h2 className="fw-bold mb-4">Beëindigde biedingen</h2>

                {loading ? (
                    <p>Laden…</p>
                ) : biedingen.length > 0 ? (
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th>Soort</th>
                                <th>Aanvoerder</th>
                                <th>Prijs</th>
                                <th>Resterend</th>
                            </tr>
                        </thead>
                        <tbody>
                            {biedingen.map(b => (
                                <tr key={b.VeilingProductId}>
                                    <td>{b.Soort}</td>
                                    <td>{b.AanvoerderNaam}</td>
                                    <td>{b.HuidigePrijs.toFixed(2)} €</td>
                                    <td>{b.ResterendeHoeveelheid}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <p>Geen beëindigde biedingen gevonden.</p>
                )}
            </div>
            <Footer />
        </>
    );
}
