using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BASE.COMMON.SECURITY
{
	/// <summary>
	/// <remarks>
	/// http://tools.ietf.org/html/draft-jones-json-web-token-10
	/// JSON Web Token (JWT) is a means of representing claims to be transferred between two parties.  
	/// The claims in a JWT are encoded as a JSON object that is digitally signed or MACed using JSON Web Signature (JWS) and/or encrypted 
	/// using JSON Web Encryption (JWE). The suggested pronunciation of JWT is the same as the English word "jot".
	/// </remarks>
	/// </summary>
    public class TokenOptions
    {
        public DateTime? expires { get; private set; }
        public DateTime? notBefore { get; private set; }
        public bool admin { get; private set; }
        public bool debug { get; private set; }

        /// <summary>
        /// Constructor.  All options are optional.
        /// </summary>
        /// <param name="notBefore">The date/time before which the token should not be considered valid. (default is now)</param>
        /// <param name="expires">The date/time at which the token should no longer be considered valid. (default is 24 hours from now)</param>
        /// <param name="admin">Set to true to bypass all security rules. (you can use this for trusted server code)</param>
        /// <param name="debug">Set to true to enable debug mode. (so you can see the results of Rules API operations)</param>
        public TokenOptions(DateTime? notBefore = null, DateTime? expires = null, bool admin = false, bool debug = false)
        {
            this.notBefore = notBefore;
            this.expires = expires;
            this.admin = admin;
            this.debug = debug;
        }
    }
}
