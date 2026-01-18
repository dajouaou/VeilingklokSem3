import { useEffect, useState } from "react";
import Navbar from "../../shared/components/Navbar";
import Footer from "../../shared/components/Footer";
import { getBeeindigdeBiedingen } from "./api/veilingPublicApi";

// Publieke pagina die beëindigde biedingen toont
export default function Veiling() {
    // State voor lijst met beëindigde biedingen
    const [biedingen, setBiedingen] = useState([]);

    // State om laadstatus bij te houden
    const [loading, setLoading] = useState(true);

    // Effect dat één keer wordt uitgevoerd bij laden van de pagina
    useEffect(() => {
        // Flag om state-updates te voorkomen na unmount
        let alive = true;

        // Asynchrone functie om data op te halen
        async function load() {
            try {
                // Beëindigde biedingen ophalen via API
                const data = await getBeeindigdeBiedingen();

                // Stop als component niet meer actief is
                if (!alive) return;

                // Zet ontvangen biedingen in state
                setBiedingen(data || []);
            } catch (e) {
                // Fout loggen voor debugging
                console.error("Fout bij laden biedingen:", e);

                // Bij fout: lege lijst tonen
                if (alive) setBiedingen([]);
            } finally {
                // Laadstatus uitzetten
                if (alive) setLoading(false);
            }
        }

        // Data ophalen starten
        load();

        // Cleanup bij unmount
        return () => { alive = false; };
    }, []);

    return (
        <>
            {/* Navigatiebalk */}
            <Navbar />

            <div className="container py-5">
                {/* Pagina titel */}
                <h2 className="fw-bold mb-4">Beëindigde biedingen</h2>

                {/* Laadstatus */}
                {loading ? (
                    <p>Laden…</p>
                ) : biedingen.length > 0 ? (
                    // Tabel met beëindigde biedingen
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
                                    <td>{b.HuidigePrijs.toFixed(2)} EUR</td>
                                    <td>{b.ResterendeHoeveelheid}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    // Geen resultaten gevonden
                    <p>Geen beëindigde biedingen gevonden.</p>
                )}
            </div>

            {/* Footer */}
            <Footer />
        </>
    );
}
