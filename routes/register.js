const express = require("express");
const bcrypt = require("bcrypt");
const pool = require("../config");

const router = express.Router();

router.post("/signup", async (req, res) => {
  try {
    const { user_fname, user_lname, email, password } = req.body;

    const hashedPassword = await bcrypt.hash(password, 10);

    const sql = `INSERT INTO users (user_fname, user_lname, email, password) VALUES (?, ?, ?, ?)`;
    await pool.query(sql, [user_fname, user_lname, email, hashedPassword]);

    return res.status(200).json({ message: "Signup successful" });
  } catch (error) {
    console.error("Error:", error);
    return res.status(500).json({ error: "An internal server error occurred" });
  }
});

module.exports = router;
