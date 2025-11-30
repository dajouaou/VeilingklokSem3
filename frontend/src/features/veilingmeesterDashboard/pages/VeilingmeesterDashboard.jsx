import { useVeilingmeesterDashboard } from "../hooks/useVeilingmeesterDashboard";
import VeilingInfo from "../components/dashboard/VeilingInfo";
import ProductCard from "../components/dashboard/ProductCard";
import QueueList from "../components/dashboard/QueueList";
import BidList from "../components/dashboard/BidList";
import AuditList from "../components/dashboard/AuditList";
import ControlPanel from "../components/dashboard/ControlPanel";

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
