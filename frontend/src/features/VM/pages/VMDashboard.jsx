import useVMDashboard from "../hooks/useVMDashboard";
import CurrentProductCard from "../components/CurrentProductCard";
import QueueList from "../components/QueueList";
import BidList from "../components/BidList";
import ControlPanel from "../components/ControlPanel";

export default function VMDashboard() {
    const {
        dashboard,
        loading,
        currentProduct,
        queue,
        bids,
        startVeiling,
        nextProduct,
        closeProduct,
    } = useVMDashboard(1);

    if (loading) return <p>Loading...</p>;

    return (
        <div>
            <h1>Veilingmeester Dashboard</h1>

            <ControlPanel
                start={startVeiling}
                next={nextProduct}
                close={closeProduct}
            />

            <CurrentProductCard product={currentProduct} />
            <QueueList queue={queue} />
            <BidList bids={bids} />
        </div>
    );
}
