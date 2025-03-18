const express = require("express");
const pool = require("../config");

const router = express.Router();

router.get("/departments", async (req, res) => {
  try {
    const sql = "SELECT * FROM department";
    const [rows] = await pool.query(sql);
    res.json(rows);
  } catch (err) {
    console.error("Error fetching courses:", err);
    res.status(500).json({ error: "Internal server error" });
  }
});

module.exports = router;
