//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: Validate.cs
//---------------------------------------------------------------

namespace CDP.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Provides helper methods to validate preconditions.
    /// </summary>
    /// <remarks>
    [DebuggerNonUserCode]
    public static class Validate
    {
        /// <summary>
        ///   Check if the argument <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The flag to be checked.</param>
        /// <param name="messageFormat">The message format string.</param>
        /// <param name="formatParameters">The message format parameters.</param>
        /// <exception cref="System.ArgumentException">
        ///   <paramref name="condition"/> is <c>false</c>.
        /// </exception>
        public static void IsTrue(bool condition, string messageFormat, params object[] formatParameters)
        {
            if (!condition)
            {
                throw new ArgumentException(string.Format(messageFormat, formatParameters));
            }
        }

        /// <summary>
        ///   Check if the <paramref name="argument"/> is not null.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="messageFormat">The message format string.</param>
        /// <param name="formatParameters">The message format parameters.</param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="argument"/> is <c>null</c>.
        /// </exception>
        public static T IsNotNull<T>(T argument, string messageFormat, params string[] formatParameters)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(string.Format(messageFormat, formatParameters));
            }

            return argument;
        }

        public static bool IsNotNull(Guid argument, string parameterName)
        {
            if (argument == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(string.Format("The argument {0} cannot be an Empty Guid.", parameterName));
            }

            return true;
        }

        /// <summary>
        ///   Check if the string <paramref name="argument"/> is not null.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="parameterName">
        ///   Name of <paramref name="argument"/> variable in the calling method.
        /// </param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="argument"/> is <c>null</c>.
        /// </exception>
        public static string IsNotNull(string argument, string parameterName) => IsNotNull(argument, "{0} is null", parameterName);

        /// <summary>
        ///   Check if the <paramref name="argument"/> is null.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="messageFormat">The message format string.</param>
        /// <param name="formatParameters">The message format parameters.</param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="argument"/> is <c>null</c>.
        /// </exception>
        public static T IsNull<T>(T argument, string messageFormat, params string[] formatParameters)
            where T : class
        {
            if (argument != null)
            {
                throw new ArgumentNullException(string.Format(messageFormat, formatParameters));
            }

            return argument;
        }

        /// <summary>
        ///   Check if a string-argument is not null and not empty.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="messageFormat">The message format string.</param>
        /// <param name="formatParameters">The message format parameters.</param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="argument"/> is <c>null</c> or empty.
        /// </exception>
        public static string IsNotNullOrEmpty(string argument, string messageFormat, params string[] formatParameters)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException(string.Format(messageFormat, (object[])formatParameters));
            }

            return argument;
        }

        /// <summary>
        /// Determines whether [is not empty] [the specified unique identifier].
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public static string IsNotNullOrEmpty(Guid guid, string parameterName)
        {
            if (guid == null)
            {
                throw new ArgumentException(string.Format("{0} is null", parameterName));
            }
            else if (guid == Guid.Empty)
            {
                throw new ArgumentException(string.Format("{0} is empty", parameterName));
            }

            return parameterName;
        }

        /// <summary>
        ///   Check if a string-argument is not <c>null</c>, not empty, and contains
        ///   non-whitespace characters.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="messageFormat">The message format string.</param>
        /// <param name="formatParameters">The message format parameters.</param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="argument"/> is <c>null</c> or empty.
        /// </exception>
        public static string IsNotNullOrWhitespace(
            string argument,
            string messageFormat,
            params string[] formatParameters)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException(string.Format(messageFormat, formatParameters));
            }

            return argument;
        }

        /// <summary>
        ///   Check if a string-argument is not <c>null</c>, not empty, and contains
        ///   non-whitespace characters.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="parameterName">
        ///   Name of <paramref name="argument"/> variable in the calling method.
        /// </param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="argument"/> is <c>null</c>, empty, or only contains whitespace
        ///   characters.
        /// </exception>
        public static string IsNotNullOrWhitespace(string argument, string parameterName) => IsNotNullOrWhitespace(argument, "{0} is null, empty, or only contains whitespace", parameterName);

        /// <summary>
        ///   Check if <paramref name="argument"/> is not negative.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="argumentName">
        ///   Name of <paramref name="argument"/> variable in the calling method.
        /// </param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="argument"/> is negative.
        /// </exception>
        public static TimeSpan IsNotNegative(TimeSpan argument, string argumentName)
        {
            if (argument < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(string.Format("Argument {0} must not be negative, was: {1}",
                    argumentName,
                    argument));
            }

            return argument;
        }

        /// <summary>
        ///   Check if the <paramref name="argument"/> is not negative.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="argumentName">
        ///   Name of <paramref name="argument"/> variable in the calling method.
        /// </param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="argument"/> is negative.
        /// </exception>
        public static double IsNotNegative(double argument, string argumentName)
        {
            if (argument < 0.0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Argument {0} must not be negative, was: {1}",
                    argumentName,
                    argument));
            }

            return argument;
        }

        /// <summary>
        /// Determines whether [is not negative] [the specified argument].
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static int IsNotNegative(int argument, string argumentName)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Argument {0} must not be negative, was: {1}",
                    argumentName,
                    argument));
            }

            return argument;
        }

        /// <summary>
        ///   Check if the <paramref name="argument"/> is greater than zero.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="argumentName">
        ///   Name of <paramref name="argument"/> variable in the calling method.
        /// </param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="argument"/> is negative.
        /// </exception>
        public static long IsPositive(long argument, string argumentName)
        {
            if (argument < 1L)
            {
                throw new ArgumentOutOfRangeException(string.Format("Argument {0} must be positive, was: {1}",
                    argumentName,
                    argument));
            }

            return argument;
        }

        /// <summary>
        /// Determines whether the specified argument is positive.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static int IsPositive(int argument, string argumentName)
        {
            if (argument < 1)
            {
                throw new ArgumentOutOfRangeException(string.Format("Argument {0} must be positive, was: {1}",
                    argumentName,
                    argument));
            }

            return argument;
        }

        /// <summary>
        ///   Checks if a numeric argument is in a specific range.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="argumentName">
        ///   Name of <paramref name="argument"/> variable in the calling method.
        /// </param>
        /// <param name="minAllowed">Minimum allowable value (inclusive).</param>
        /// <param name="maxAllowed">Maximum allowable value (inclusive).</param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="argument"/> is less than <paramref name="minAllowed"/>, or it
        ///   is greater than <paramref name="maxAllowed"/>.
        /// </exception>
        public static int IsInRange(int argument, string argumentName, int minAllowed, int maxAllowed)
        {
            if (argument < minAllowed || argument > maxAllowed)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Argument {0} must be in the interval: [{1}, {2}]. Was: {3}",
                    argumentName,
                    minAllowed,
                    maxAllowed,
                    argument));
            }

            return argument;
        }

        /// <summary>
        ///   Checks if a numeric argument is in a specific range.
        /// </summary>
        /// <param name="argument">The argument to be tested.</param>
        /// <param name="argumentName">
        ///   Name of <paramref name="argument"/> variable in the calling method.
        /// </param>
        /// <param name="minAllowed">Minimum allowable value (inclusive).</param>
        /// <param name="maxAllowed">Maximum allowable value (inclusive).</param>
        /// <returns><paramref name="argument"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="argument"/> is less than <paramref name="minAllowed"/>, or it
        ///   is greater than <paramref name="maxAllowed"/>.
        /// </exception>
        public static long IsInRange(long argument, string argumentName, long minAllowed, long maxAllowed)
        {
            if (argument < minAllowed || argument > maxAllowed)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Argument {0} must be in the interval: [{1}, {2}]. Was: {3}",
                    argumentName,
                    minAllowed,
                    maxAllowed,
                    argument));
            }

            return argument;
        }

        /// <summary>
        ///   Checks if the length of an array is in a specific range.
        /// </summary>
        /// <param name="array">The array to be tested.</param>
        /// <param name="arrayName">The name of the <paramref name="array"/> in the calling method.</param>
        /// <param name="minAllowed">Minimum length of the <paramref name="array"/> (inclusive).</param>
        /// <param name="maxAllowed">Maximum length of the <paramref name="array"/> (inclusive).</param>
        /// <returns><paramref name="array"/></returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="array"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The length of the <paramref name="array"/> is less than <paramref name="minAllowed"/>, or it
        ///   is greater than <paramref name="maxAllowed"/>.
        /// </exception>
        public static T[] IsLengthInRange<T>(T[] array, string arrayName, int minAllowed, int maxAllowed)
        {
            Validate.IsNotNull<T[]>(array, arrayName);
            if (array.Length < minAllowed || array.Length > maxAllowed)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Array {0}'s length must be in the interval: [{1}, {2}]. Was: {3}",
                    arrayName,
                    minAllowed,
                    maxAllowed,
                    array.Length));
            }

            return array;
        }

        /// <summary>
        ///   Checks if the count of a collection is in the specific range.
        /// </summary>
        /// <param name="collection">The collection to be tested.</param>
        /// <param name="collectionName">The name of the <paramref name="collection"/> in the calling method.</param>
        /// <param name="minAllowed">Minimum length of the <paramref name="collection"/> (inclusive).</param>
        /// <param name="maxAllowed">Maximum length of the <paramref name="collection"/> (inclusive).</param>
        /// <returns><paramref name="collection"/></returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The count of the <paramref name="collection"/> is less than <paramref name="minAllowed"/>, or it
        ///   is greater than <paramref name="maxAllowed"/>.
        /// </exception>
        public static ICollection<T> IsCountInRange<T>(ICollection<T> collection, string collectionName, int minAllowed, int maxAllowed)
        {
            Validate.IsNotNull<ICollection<T>>(collection, collectionName);

            if (collection.Count < minAllowed || collection.Count > maxAllowed)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Collection {0}'s count must be in the interval: [{1}, {2}]. Was: {3}",
                    collectionName,
                    minAllowed,
                    maxAllowed,
                    collection.Count));
            }

            return collection;
        }

        /// <summary>
        /// Determines whether [is enum size in range] [the specified enumerable].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="enumerableName">Name of the enumeration.</param>
        /// <param name="minAllowed">The minimum allowed.</param>
        /// <param name="maxAllowed">The maximum allowed.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> IsEnumerableSizeInRange<T>(IEnumerable<T> enumerable, string enumerableName, int minAllowed, int maxAllowed)
        {
            if (enumerable.Count() > maxAllowed || enumerable.Count() < minAllowed)
            {
                throw new ArgumentOutOfRangeException(string.Format("The parameter {0}'s count must be in the interval: [{1}, {2}]. Was {3}",
                    enumerableName,
                    minAllowed,
                    maxAllowed,
                    enumerable.Count()));
            }

            return enumerable;
        }

        /// <summary>
        ///   Tests if <paramref name="expected"/> and <paramref name="actual"/> values are equal.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="messageFormat">The message format string.</param>
        /// <param name="formatParameters">The message format parameters.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="expected"/> and <paramref name="actual"/> values are not equal.
        /// </exception>
        public static void AreEqual(long expected, long actual, string messageFormat, params string[] formatParameters) => 
            IsTrue(expected == actual, string.Format("[Failed {0}=={1}]: {2}", expected, actual, messageFormat), formatParameters);
    }
}