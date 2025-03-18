const express = require("express");
const pool = require("../config");

const router = express.Router();

router.get("/department/:department_id/courses", async (req, res) => {
  try {
    const department_id = req.params.department_id;
    const sql =
      "SELECT c.course_id, c.course_name FROM Courses c JOIN Course_Department cd ON c.course_id = cd.course_id WHERE cd.department_id = ?";
    const [rows] = await pool.query(sql, [department_id]);
    res.json(rows);
  } catch (err) {
    console.error("Error fetching courses:", err);
    res.status(500).json({ error: "Internal server error" });
  }
});

module.exports = router;
