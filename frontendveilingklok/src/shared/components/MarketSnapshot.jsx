export default function MarketSnapshot() {
    return (
        <section className="market-snapshot py-4">
            <div className="container">
                <div className="snapshot-card">
                    <div className="row g-3 align-items-center">
                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Status</div>
                                <div className="kpi-value">
                                    <span className="badge-live">LIVE</span>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Online bieders</div>
                                <div className="kpi-value">3</div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Laatste prijs</div>
                                <div className="kpi-value">40.99</div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Latency SLA</div>
                                <div className="kpi-value">&lt; 200 ms</div>
                                <div className="kpi-sub">Conform NFR</div>
                            </div>
                        </div>
                    </div>
                </div>


            </div>
        </section>
    );
}