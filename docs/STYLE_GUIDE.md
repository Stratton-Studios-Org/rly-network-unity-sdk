# Rally Protocol Documentation Style Guide

This style guide provides guidelines for maintaining consistent, high-quality documentation for the Rally Protocol Unity SDK. All contributors should follow these guidelines when creating or updating documentation.

## General Principles

1. **Be Clear and Concise**: Use simple, direct language. Avoid jargon when possible, and define technical terms when they first appear.

2. **Be Accurate**: Ensure all information is technically accurate and up-to-date.

3. **Be Comprehensive**: Cover all essential aspects of the topic, but don't overwhelm with unnecessary details.

4. **Be Consistent**: Follow the conventions in this guide for formatting, terminology, and structure.

## Document Structure

### Headings

Use the following heading structure:

```markdown
# Document Title (H1)

## Major Section (H2)

### Subsection (H3)

#### Minor Subsection (H4)
```

- Use only one H1 heading per document, at the top.
- Use sentence case for headings (capitalize only the first word and proper nouns).
- Don't skip heading levels (e.g., don't go from H2 to H4).

### Document Sections

Include these standard sections in most documents:

1. **Introduction/Overview**: Brief explanation of what the document covers
2. **Prerequisites**: Required knowledge, tools, or setup
3. **Main Content**: Core information divided into logical sections
4. **Examples**: Practical code examples
5. **Best Practices**: Recommended approaches
6. **Troubleshooting**: Common issues and solutions (if applicable)
7. **Next Steps**: Related documentation or further learning

## Code Examples

### Code Blocks

Use fenced code blocks with the appropriate language identifier:

````markdown
```csharp
public class Example
{
    public void DoSomething()
    {
        Debug.Log("Hello World");
    }
}
```
````

### Standard Patterns

Standardize how you access the Rally Protocol services:

```csharp
// Preferred approach: Store a reference in a field/property
private IRallyNetwork rlyNetwork;

private void Awake()
{
    rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
}

// For one-off access, use the full path
string address = RallyUnityManager.Instance.RlyNetwork.AccountManager.GetAccountAddress();
```

### Error Handling

Always include error handling in code examples:

```csharp
public async Task<bool> ExampleFunction()
{
    try
    {
        // Function code here
        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error in ExampleFunction: {e.Message}");
        return false;
    }
}
```

## Formatting Guidelines

### Terminology

Use consistent terminology throughout:

- **RLY token**: Refers to the Rally Protocol token
- **Rally Protocol**: The full name of the protocol
- **Account**: A user's blockchain account
- **Wallet**: The collection of accounts a user manages
- **Transaction**: An operation recorded on the blockchain

### Formatting Conventions

- **Bold** for UI elements and important terms: Click the **Create Account** button
- *Italic* for emphasis: It is *essential* to secure your private key
- `Code` for inline code, filenames, and technical terms: Use the `IRallyNetwork` interface

### Lists

Use:

- Bulleted lists for unordered items
- Numbered lists for sequential steps
- Nested lists for hierarchical information

### Tables

Use tables for comparing multiple items or presenting structured data:

| Feature | Description | Example |
|---------|-------------|---------|
| Account Management | Create and manage accounts | `AccountManager.CreateAccountAsync()` |
| Token Transfers | Send tokens between accounts | `rlyNetwork.TransferAsync(address, amount)` |

## Images and Diagrams

- Use images sparingly and only when they add significant value
- Provide alt text for all images
- Use SVG format when possible for diagrams
- Include sequence diagrams for complex interactions

## Cross-Referencing

Use relative links to reference other documentation:

```markdown
See the [Account Creation](../examples/account-creation.md) example for more details.
```

## Version Information

- Indicate version compatibility clearly
- Note breaking changes between versions
- Use this format for version references: Rally Protocol SDK v1.0.0

## Document Updates

When updating documentation:

1. Keep a changelog at the bottom of significantly changed documents
2. Update related documentation that might be affected
3. Review all examples to ensure they still work with the current version

## Examples of Good Documentation

Here are some examples that demonstrate good documentation practices:

### Good Introductory Paragraph

```markdown
# Account Management

The Account Management API provides methods for creating, importing, and managing blockchain accounts in your Unity project. These accounts are used for all blockchain operations including token transfers, NFT management, and contract interactions.
```

### Good Code Example with Comments

````markdown
```csharp
// Create a new account with cloud backup
public async Task<bool> CreateNewAccount()
{
    try
    {
        // Initialize account options
        AccountOptions options = new AccountOptions
        {
            Overwrite = false, // Don't overwrite existing accounts
            StorageOptions = new StorageOptions
            {
                SaveToCloud = true, // Enable cloud backup
                RejectOnCloudSaveFailure = false // Continue even if cloud save fails
            }
        };
        
        // Create the account
        bool success = await RallyUnityManager.Instance.RlyNetwork.AccountManager
            .CreateAccountAsync(options);
            
        if (success)
        {
            Debug.Log("Account created successfully");
        }
        
        return success;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error creating account: {e.Message}");
        return false;
    }
}
```
````

## Quality Checklist

Use this checklist before submitting documentation:

- [ ] Document follows the heading structure guidelines
- [ ] All code examples are complete, correct, and follow error handling practices
- [ ] Terminology is consistent with the style guide
- [ ] Links to other documents are working
- [ ] No spelling or grammatical errors
- [ ] Examples are up-to-date with the current SDK version
- [ ] Document includes best practices section
- [ ] Troubleshooting information is included if relevant

## Tone and Voice

- Write in a professional but approachable tone
- Use second person ("you") when addressing the reader
- Avoid unnecessary humor or colloquialisms
- Be encouraging and supportive, especially in troubleshooting sections

## Contribution Process

When contributing to documentation:

1. Follow this style guide
2. Create a separate branch for your changes
3. Submit a pull request with a clear description of your updates
4. Respond promptly to review feedback
