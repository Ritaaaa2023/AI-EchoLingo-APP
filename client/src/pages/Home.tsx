import { Button } from "../components/ui/button";
import { Link } from "react-router-dom";
import { motion } from "framer-motion";

export default function Home() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-orange-50 px-4 py-20">
      <motion.div
        initial={{ opacity: 0, y: 40 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.6 }}
        className="text-center max-w-2xl"
      >
        <h1 className="text-4xl md:text-6xl font-extrabold text-orange-600 mb-4 leading-tight">
          Practice Real English With AI
        </h1>
        <p className="text-lg md:text-xl text-gray-700 mb-8">
          EchoLingo helps you speak naturally and confidently through
          journaling, translation, and pronunciation practice.
        </p>

        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Link to="/register">
            <Button className="bg-orange-500 hover:bg-orange-600 text-white px-6 py-3 text-lg rounded-xl shadow-md">
              Get Started for Free
            </Button>
          </Link>
          <Link to="/login">
            <Button
              variant="outline"
              className="text-orange-600 border-orange-400 hover:bg-orange-100 px-6 py-3 text-lg rounded-xl"
            >
              I Already Have an Account
            </Button>
          </Link>
        </div>
      </motion.div>
    </div>
  );
}
