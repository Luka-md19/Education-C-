const express = require("express");
const passport = require("passport");
const pool = require("../config");
const bcrypt = require("bcrypt");
const router = express.Router();

// Local authentication
router.post("/login", async (req, res) => {
  const email = req.body.email;
  const password = req.body.password;

  try {
    const [user] = await pool.query("SELECT * FROM users WHERE email = ?", [
      email,
    ]);

    if (!user.length) {
      return res.status(401).json({ message: "User not found" });
    }

    const passwordMatch = await bcrypt.compare(password, user[0].password);

    if (!passwordMatch) {
      return res.status(401).json({ message: "Invalid password" });
    }

    // Manually authenticate the user
    // You can customize the user object based on your needs
    const authenticatedUser = {
      email: user[0].email,
      password: user[0].password,
      profile_photo: user[0].profile_photo,
    };

    // Serialize the user into the session
    req.login(authenticatedUser, (err) => {
      if (err) {
        return res.status(500).json({ error: "Authentication error" });
      }
      return res.status(200).json({ message: "Signin successful", user });
    });
  } catch (error) {
    console.error("Error:", error);
    return res.status(500).json({ error: "An internal server error occurred" });
  }
});

//Google authentication
router.get(
  "/auth/google",
  passport.authenticate("google", { scope: ["email", "profile"] })
);

router.get(
  "/auth/google/callback",
  passport.authenticate("google", {
    successRedirect: "http://localhost:5173/",
    failureRedirect: "http://localhost:5173/signin",
  }),
  (req, res) => {
    // Set isLoggedIn to true in session or whatever your state management is
    req.session.isLoggedIn = true;

    // Send response indicating successful login
    res.json({ isLoggedIn: true });
  }
);

// Facebook authentication
router.get(
  "/facebook",
  passport.authenticate("facebook", { scope: ["public_profile", "email"] })
);
router.get(
  "/facebook/callback",
  passport.authenticate("facebook", {
    successRedirect: "http://localhost:5173/",
    failureRedirect: "http://localhost:5173/signin",
  })
);

router.post("/logout", function (req, res, next) {
  req.logout(function (err) {
    if (err) {
      return next(err);
    }
    res.redirect("/");
  });
});

module.exports = router;
