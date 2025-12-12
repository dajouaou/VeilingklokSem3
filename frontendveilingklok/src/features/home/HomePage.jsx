import Navbar from "../../shared/components/Navbar";
import HeroBanner from "../../shared/components/HeroBanner";
import NextBids from "../../shared/components/NextBids";
import ReviewSection from "../../shared/components/ReviewSection";
import Footer from "../../shared/components/Footer";

export default function HomePage() {
    return (
        <>
            <Navbar />

            <HeroBanner />
            <NextBids />
            <ReviewSection />
            <Footer />
        </>
    );

}

