const express = require("express");
const bodyParser = require("body-parser");
const cors = require("cors");
const passport = require("passport");
const session = require("express-session");
const LocalStrategy = require("passport-local");
const GoogleStrategy = require("passport-google-oauth20");
const FacebookStrategy = require("passport-facebook");
const pool = require("./config");
const bcrypt = require("bcrypt");

const app = express();
app.use(express.json());
app.use(bodyParser.json());
app.use(cors());
const port = 5000;

const secretKey = process.env.SECRET_KEY;

// Middleware
app.use(express.urlencoded({ extended: true }));
app.use(
  session({ secret: `${secretKey}`, resave: false, saveUninitialized: false })
);
app.use(passport.initialize());
app.use(passport.session());

const GOOGLE_CLIENT_SECRET = process.env.GOOGLE_CLIENT_SECRET;
const FACEBOOK_APP_SECRET = process.env.FACEBOOK_APP_SECRET;

// Load routes
const LoginRoutes = require("./routes/login");
const corusesRoutes = require("./routes/courses");
const departmentRoutes = require("./routes/department");
const registerRoutes = require("./routes/register");

passport.use(
  new LocalStrategy(
    {
      usernameField: "email",
      passwordField: "password",
      passReqToCallback: true,
    },
    (email, password, done) => {
      pool.query("SELECT * FROM users WHERE email = ?", [email], (err, row) => {
        if (err) {
          return done(err);
        }
        if (!row) {
          return done(null, false, { message: "Incorrect Email or password." });
        }
        const user = row[0];

        const passwordMatch = bcrypt.compare(password, user[0].password);

        if (!passwordMatch) {
          return done(null, false, {
            message: "Incorrect password or Email.",
          });
        }
        return done(null, row);
      });
    }
  )
);

passport.use(
  new GoogleStrategy(
    {
      clientID: process.env.GOOGLE_CLIENT_ID,
      clientSecret: GOOGLE_CLIENT_SECRET,
      callbackURL: "http://localhost:5000/api/auth/google/callback",
      userProfileURL: "https://www.googleapis.com/oauth2/v3/userinfo",
    },
    async (accessToken, refreshToken, profile, done) => {
      console.log(profile);
      try {
        // Search for a user with the given Google ID
        const [rows] = await pool.query(
          "SELECT * FROM users WHERE google_id = ?",
          [profile.id]
        );
        let user = rows[0];

        if (!user) {
          // If user not found, insert a new user
          const insertResult = await pool.query(
            "INSERT INTO users (google_id, user_fname, user_lname, email, profile_photo) VALUES (?,?,?,?,?)",
            [
              profile.id,
              profile.name.givenName,
              profile.name.familyName,
              profile.emails[0].value,
              profile.photos[0].value,
            ]
          );
          const [rows] = await pool.query(
            "SELECT * FROM users WHERE google_id = ?",
            [profile.id]
          );
          let user = rows[0];
        }
        return done(null, user);
      } catch (error) {
        return done(error, false);
      }
    }
  )
);

passport.use(
  new FacebookStrategy(
    {
      clientID: process.env.FACEBOOK_APP_ID,
      clientSecret: FACEBOOK_APP_SECRET,
      callbackURL: "http://localhost:5000/api/facebook/callback",
    },
    async (accessToken, refreshToken, profile, done) => {
      console.log(profile);
      try {
        // Search for a user with the given Google ID
        const [rows] = await pool.query(
          "SELECT * FROM users WHERE facebook_id = ?",
          [profile.id]
        );
        let user = rows[0];

        if (!user) {
          const nameParts = profile.displayName.split(" ");
          const lname = nameParts.pop();
          const fname = nameParts.join(" ");

          const insertResult = await pool.query(
            "INSERT INTO users (facebook_id, user_fname, user_lname) VALUES (?,?,?)",
            [profile.id, fname, lname]
          );
          const [rows] = await pool.query(
            "SELECT * FROM users WHERE facebook_id = ?",
            [profile.id]
          );
          let user = rows[0];
        }
        return done(null, user);
      } catch (error) {
        return done(error, false);
      }
    }
  )
);

// Serialize user for session
passport.serializeUser((user, done) => {
  done(null, user);
});
// Deserialize user from session
passport.deserializeUser((user, done) => {
  done(null, user);
});

app.use("/api", departmentRoutes);

app.use("/api", corusesRoutes);

app.use("/api", registerRoutes);

app.use("/api", LoginRoutes);

app.listen(port, () => {
  console.log(`Backend server is listening on port ${port}`);
});
