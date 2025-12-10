import {
    startGeplandeVeiling,
    fetchBeschikbareProducten
} from "../api/veilingApi";


export default function PlanVeiling() {

    const [producten, setProducten] = useState([]);

    const [datum, setDatum] = useState("");
    const [startTijd, setStartTijd] = useState("09:00");
    const [eindTijd, setEindTijd] = useState("11:00");
    const [productId, setProductId] = useState("");

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    useEffect(() => {
        async function load() {
            try {
                const producten = await fetchBeschikbareProducten();
                setProducten(producten);
            } catch (e) {
                console.error(e);
                setError("Kon producten niet laden.");
            } finally {
                setLoading(false);
            }
        }
        load();
    }, []);

    async function handlePlan() {
        setError("");
        setSuccess("");

        if (!datum || !productId) {
            setError("Kies een datum en een product.");
            return;
        }

        try {
            await startGeplandeVeiling({
                datum,
                startTijd,
                eindTijd,
                productId
            });

            setSuccess("Veiling succesvol gepland!");
        } catch (e) {
            console.error(e);
            setError("Fout bij het plannen van de veiling.");
        }
    }

    return (
        <div className="vm-card p-4">
            <h2>Veiling plannen</h2>

            {loading && <p>Laden…</p>}
            {error && <div className="alert alert-danger">{error}</div>}
            {success && <div className="alert alert-success">{success}</div>}

            {!loading && (
                <div className="row g-3 mt-3">

                    {/* Datum */}
                    <div className="col-md-4">
                        <label>Veildatum</label>
                        <input
                            type="date"
                            className="form-control"
                            value={datum}
                            onChange={(e) => setDatum(e.target.value)}
                        />
                    </div>

                    {/* Starttijd */}
                    <div className="col-md-4">
                        <label>Starttijd</label>
                        <input
                            type="time"
                            className="form-control"
                            value={startTijd}
                            onChange={(e) => setStartTijd(e.target.value)}
                        />
                    </div>

                    {/* Eindtijd */}
                    <div className="col-md-4">
                        <label>Eindtijd</label>
                        <input
                            type="time"
                            className="form-control"
                            value={eindTijd}
                            onChange={(e) => setEindTijd(e.target.value)}
                        />
                    </div>

                    {/* Product dropdown */}
                    <div className="col-md-12 mt-3">
                        <label>Product</label>
                        <select
                            className="form-select"
                            value={productId}
                            onChange={(e) => setProductId(e.target.value)}
                        >
                            <option value="">-- Kies product --</option>
                            {producten.map((p) => (
                                <option key={p.id} value={p.id}>
                                    {p.soort} ({p.hoeveelheid} stuks) – {p.aanvoerderNaam}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="col-md-12 text-end mt-3">
                        <button className="btn btn-primary" onClick={handlePlan}>
                            Veiling plannen
                        </button>
                    </div>

                </div>
            )}
        </div>
    );
}
