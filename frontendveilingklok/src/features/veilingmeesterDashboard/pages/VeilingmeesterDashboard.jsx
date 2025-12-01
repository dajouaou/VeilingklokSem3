import { useVeilingmeesterDashboard } from "../hooks/useVeilingmeesterDashboard";
import VeilingInfo from "../components/VeilingInfo.jsx";
import ProductCard from "../components/ProductCard.jsx";
import QueueList from "../components/QueueList.jsx";
import BidList from "../components/BidList";
import AuditList from "../components/AuditList";
import ControlPanel from "../components/ControlPanel";

export default function VeilingmeesterDashboard() {
    const veilingId = 1; // of via URL param
    const dm = useVeilingmeesterDashboard(veilingId);

    return (
        <div className="dashboard-page">
            <div className="left-col">
                <VeilingInfo details={dm.details} />
                <ProductCard lot={dm.currentLot} />
                <ControlPanel actions={dm} />
            </div>

            <div className="right-col">
                <QueueList queue={dm.queue} />
                <BidList bids={dm.bids} />
                <AuditList audit={dm.audit} />
            </div>
        </div>
    );
}
