export default function VeilingInfo({ details }) {
    if (!details) return <div className="card">Loading...</div>;

    return (
        <div className="card">
            <h2>Veiling #{details.id}</h2>
            <p>Status: <strong>{details.status}</strong></p>
            <p>Start: {details.startTijdUtc}</p>
            <p>Einde: {details.eindTijdUtc}</p>
            {details.currentProductNaam && (
                <p>Huidig product: {details.currentProductNaam}</p>
            )}
        </div>
    );
}

